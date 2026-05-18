using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Data.Repositories;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using SPMD.Hubs;

namespace SPMD.Services
{
    public class PrescriptionService : BaseService<Prescription>
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PrescriptionService(IRepository<Prescription> repository, AppDbContext context, IHubContext<NotificationHub> hubContext) : base(repository)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ValidationResult> ValidatePrescriptionAsync(Prescription prescription)
        {
            var result = new ValidationResult();

            // 1. Fetch Patient with Allergies and Active Prescriptions
            var patient = await _context.Patients
                .Include(p => p.AllergyRecords)
                .ThenInclude(a => a.Medicine)
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Items)
                .ThenInclude(pi => pi.Medicine)
                .FirstOrDefaultAsync(p => p.PatientId == prescription.PatientId);

            if (patient == null)
            {
                result.Errors.Add("Patient not found.");
                result.IsValid = false;
                return result;
            }

            // 2. Allergy Check
            foreach (var item in prescription.Items)
            {
                // We need to fetch the medicine details if not included
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine == null) continue;

                var matchingAllergy = patient.AllergyRecords.FirstOrDefault(a => 
                    (a.MedicineId == medicine.MedicineId) || 
                    (!string.IsNullOrEmpty(a.Substance) && medicine.MedicineName.Contains(a.Substance, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.Substance) && medicine.GenericName != null && medicine.GenericName.Contains(a.Substance, StringComparison.OrdinalIgnoreCase))
                );

                if (matchingAllergy != null)
                {
                    string message = $"Allergy Warning: Patient is allergic to {medicine.MedicineName} (Substance: {matchingAllergy.Substance}). Severity: {matchingAllergy.Severity}.";
                    if (matchingAllergy.Severity == SeverityLevel.Severe)
                    {
                        result.Errors.Add(message);
                        result.IsValid = false;
                    }
                    else
                    {
                        result.Warnings.Add(message);
                        result.RequiresOverride = true;
                    }
                }
            }

            // 3. Duplicate Therapy & Drug-Drug Interaction Checks
            var activePrescriptions = patient.Prescriptions
                .Where(p => p.Status == PrescriptionStatus.Issued || p.Status == PrescriptionStatus.Dispensed)
                .ToList();

            var activeItems = activePrescriptions.SelectMany(p => p.Items).ToList();
            var allInteractions = await _context.DrugInteractions.ToListAsync();

            // Prepare list of medicines in the new prescription
            var newMedicines = new List<Medicine>();
            foreach (var item in prescription.Items)
            {
                var med = await _context.Medicines.FindAsync(item.MedicineId);
                if (med != null) newMedicines.Add(med);
            }

            foreach (var newMed in newMedicines)
            {
                // A. Check against Active Medications (Duplicates & Interactions)
                foreach (var activeItem in activeItems)
                {
                    var activeMed = activeItem.Medicine;
                    if (activeMed == null) continue;

                    // 1. Duplicate Check
                    if (activeMed.MedicineId == newMed.MedicineId)
                    {
                        result.Warnings.Add($"Duplicate Therapy: Patient is already prescribed {newMed.MedicineName} in Prescription #{activeItem.Prescription.PrescriptionNumber}.");
                        result.RequiresOverride = true;
                    }
                    else if (!string.IsNullOrEmpty(newMed.GenericName) && !string.IsNullOrEmpty(activeMed.GenericName) && 
                             newMed.GenericName.Equals(activeMed.GenericName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Warnings.Add($"Generic Duplicate: Patient is taking {activeMed.MedicineName}, which has the same generic name ({newMed.GenericName}) as {newMed.MedicineName}.");
                        result.RequiresOverride = true;
                    }

                    // 2. Interaction Check (New med vs Active med)
                    CheckAndAddInteraction(newMed, activeMed, allInteractions, result);
                }
            }

            // B. Check interactions among items in the NEW prescription
            for (int i = 0; i < newMedicines.Count; i++)
            {
                for (int j = i + 1; j < newMedicines.Count; j++)
                {
                    CheckAndAddInteraction(newMedicines[i], newMedicines[j], allInteractions, result);
                }
            }

            return result;
        }

        private void CheckAndAddInteraction(Medicine medA, Medicine medB, List<DrugInteraction> interactions, ValidationResult result)
        {
            if (string.IsNullOrEmpty(medA.GenericName) || string.IsNullOrEmpty(medB.GenericName)) return;

            var interaction = interactions.FirstOrDefault(i =>
                (i.GenericNameA.Equals(medA.GenericName, StringComparison.OrdinalIgnoreCase) && i.GenericNameB.Equals(medB.GenericName, StringComparison.OrdinalIgnoreCase)) ||
                (i.GenericNameA.Equals(medB.GenericName, StringComparison.OrdinalIgnoreCase) && i.GenericNameB.Equals(medA.GenericName, StringComparison.OrdinalIgnoreCase))
            );

            if (interaction != null)
            {
                string message = $"Drug Interaction: {medA.MedicineName} and {medB.MedicineName}. {interaction.Description} Severity: {interaction.Severity}.";
                if (interaction.Severity == SeverityLevel.Severe)
                {
                    result.Errors.Add(message);
                    result.IsValid = false;
                }
                else
                {
                    result.Warnings.Add(message);
                    result.RequiresOverride = true;
                }
            }
        }

        public async Task<Prescription> IssuePrescriptionAsync(Prescription prescription)
        {
            var validation = await ValidatePrescriptionAsync(prescription);
            if (!validation.IsValid)
            {
                throw new Exception("Prescription validation failed: " + string.Join(", ", validation.Errors));
            }

            prescription.Status = PrescriptionStatus.Issued;
            prescription.IssuedAt = DateTime.UtcNow;

            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();

            // Notify pharmacists about new prescription
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "System", $"New Prescription Issued: #{prescription.PrescriptionNumber.Substring(0, 8)} for Patient #{prescription.PatientId}");

            return prescription;
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsForPatientAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                .ThenInclude(pi => pi.Medicine)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.IssuedAt)
                .ToListAsync();
        }
    }
}
