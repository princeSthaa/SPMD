using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using SPMD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Dispensing
{
    [Authorize(Roles = "Doctor,Pharmacist")]
    public class DirectModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly DispensingService _dispensingService;
        private readonly PrescriptionService _prescriptionService;

        public DirectModel(AppDbContext context, DispensingService dispensingService, PrescriptionService prescriptionService)
        {
            _context = context;
            _dispensingService = dispensingService;
            _prescriptionService = prescriptionService;
        }

        public SPMD.Models.Patient? SelectedPatient { get; set; }
        public SelectList? MedicineList { get; set; }

        [BindProperty]
        public List<PrescriptionItem> Items { get; set; } = new() { new PrescriptionItem { Quantity = 1 } };

        public ValidationResult? ValidationResult { get; set; }

        public async Task<IActionResult> OnGetAsync(int patientId)
        {
            SelectedPatient = await _context.Patients
                .Include(p => p.AllergyRecords)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (SelectedPatient == null) return RedirectToPage("/Patients/Search");

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDispenseAsync(int patientId)
        {
            SelectedPatient = await _context.Patients.FindAsync(patientId);
            await LoadDataAsync();

            var validItems = Items.Where(i => i.MedicineId != 0).ToList();
            if (!validItems.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one medication is required.");
                return Page();
            }

            // Safety Validation
            var tempPrescription = new Prescription { PatientId = patientId, Items = validItems };
            ValidationResult = await _prescriptionService.ValidatePrescriptionAsync(tempPrescription);

            if (!ValidationResult.IsValid) return Page();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == int.Parse(userIdClaim));
            var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(ph => ph.UserId == int.Parse(userIdClaim));

            // We need a doctor ID and a pharmacist ID for the record. 
            // In direct mode, if a doctor does it, we might use a default system pharmacist or the doctor themselves if they have both roles.
            // For this prototype, we'll try to find any pharmacist if the user is a doctor.
            int docId = doctor?.DoctorId ?? (await _context.Doctors.Select(d => d.DoctorId).FirstOrDefaultAsync());
            int pharmId = pharmacist?.PharmacistId ?? (await _context.Pharmacists.Select(p => p.PharmacistId).FirstOrDefaultAsync());

            if (docId == 0 || pharmId == 0)
            {
                ModelState.AddModelError(string.Empty, "Incomplete clinical profiles detected. Both an authorized Doctor and Pharmacist are required for direct fulfillment.");
                return Page();
            }

            try
            {
                bool success = await _dispensingService.DirectDispenseAsync(patientId, docId, pharmId, validItems, userIdClaim);
                if (success)
                {
                    return RedirectToPage("./Queue", new { success = true, message = "Medication dispensed directly." });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Dispensing Error: {ex.Message}");
            }

            return Page();
        }

        private async Task LoadDataAsync()
        {
            var medicines = await _context.Medicines.OrderBy(m => m.MedicineName).ToListAsync();
            MedicineList = new SelectList(medicines, "MedicineId", "MedicineName");
        }
    }
}
