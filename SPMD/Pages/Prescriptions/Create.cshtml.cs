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

namespace SPMD.Pages.Prescriptions
{
    [Authorize(Roles = "SuperAdmin,Doctor")]
    public class CreateModel : PageModel
    {
        private readonly PrescriptionService _prescriptionService;
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;

        public CreateModel(PrescriptionService prescriptionService, AppDbContext context, AuditService auditService)
        {
            _prescriptionService = prescriptionService;
            _context = context;
            _auditService = auditService;
        }

        [BindProperty]
        public Prescription Prescription { get; set; } = new();

        [BindProperty]
        public List<PrescriptionItem> Items { get; set; } = new() { new PrescriptionItem() };

        public SPMD.Models.Patient? SelectedPatient { get; set; }
        public SelectList? MedicineList { get; set; }
        public ValidationResult? ValidationResult { get; set; }

        public async Task<IActionResult> OnGetAsync(int? patientId)
        {
            if (patientId == null) return RedirectToPage("/Patients/Search");

            SelectedPatient = await _context.Patients
                .Include(p => p.AllergyRecords)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (SelectedPatient == null) return RedirectToPage("/Patients/Search");

            Prescription.PatientId = patientId.Value;
            
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostValidateAsync()
        {
            await LoadDataAsync();
            Prescription.Items = Items.Where(i => i.MedicineId != 0).ToList();
            
            ValidationResult = await _prescriptionService.ValidatePrescriptionAsync(Prescription);
            
            // Log the validation attempt
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _auditService.LogActionAsync(userId ?? "0", "Doctor", AuditAction.Validate, "Prescription", "0", "Safety check performed.");

            return Page();
        }

        public async Task<IActionResult> OnPostIssueAsync(string? overrideReason)
        {
            await LoadDataAsync();
            Prescription.Items = Items.Where(i => i.MedicineId != 0).ToList();

            if (!Prescription.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one medication item is required.");
                return Page();
            }

            var validation = await _prescriptionService.ValidatePrescriptionAsync(Prescription);
            
            if (!validation.IsValid)
            {
                ValidationResult = validation;
                return Page();
            }

            // Enforce Clinical Justification for warnings
            if (validation.Warnings.Any() && string.IsNullOrWhiteSpace(overrideReason))
            {
                ModelState.AddModelError(string.Empty, "Clinical justification is required to override safety warnings.");
                ValidationResult = validation;
                return Page();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null)
                {
                    ModelState.AddModelError(string.Empty, "Doctor profile not found.");
                    return Page();
                }

                Prescription.DoctorId = doctor.DoctorId;
                Prescription.Status = PrescriptionStatus.Issued;
                Prescription.IssuedAt = DateTime.UtcNow;
                Prescription.PrescriptionNumber = Guid.NewGuid().ToString().ToUpper().Substring(0, 12);

                if (!string.IsNullOrEmpty(overrideReason))
                {
                    Prescription.Notes = $"[CLINICAL OVERRIDE: {overrideReason}]\n" + (Prescription.Notes ?? "");
                }

                await _prescriptionService.CreateAsync(Prescription);
                
                await _auditService.LogActionAsync(userIdClaim, "Doctor", AuditAction.Create, "Prescription", Prescription.PrescriptionId.ToString(), $"Prescription issued. Override applied: {!string.IsNullOrEmpty(overrideReason)}");

                return RedirectToPage("/Doctor/Index");
            }

            return Page();
        }

        private async Task LoadDataAsync()
        {
            SelectedPatient = await _context.Patients
                .Include(p => p.AllergyRecords)
                .FirstOrDefaultAsync(p => p.PatientId == Prescription.PatientId);

            var medicines = await _context.Medicines.OrderBy(m => m.MedicineName).ToListAsync();
            MedicineList = new SelectList(medicines, "MedicineId", "MedicineName");
        }
    }
}
