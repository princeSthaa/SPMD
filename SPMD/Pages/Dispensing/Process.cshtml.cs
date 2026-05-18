using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using SPMD.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Dispensing
{
    [Authorize(Roles = "Pharmacist,Admin,Doctor")]
    public class ProcessModel : PageModel

    {
        private readonly AppDbContext _context;
        private readonly DispensingService _dispensingService;

        public ProcessModel(AppDbContext context, DispensingService dispensingService)
        {
            _context = context;
            _dispensingService = dispensingService;
        }

        public Prescription? Prescription { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                .ThenInclude(pi => pi.Medicine)
                .ThenInclude(m => m.Batches)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (Prescription == null || Prescription.Status != PrescriptionStatus.Issued)
            {
                return RedirectToPage("./Queue");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                .ThenInclude(pi => pi.Medicine)
                .ThenInclude(m => m.Batches)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null) return NotFound();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            int userId = int.Parse(userIdClaim);

            var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(ph => ph.UserId == userId);
            
            // If the user is a doctor, they might not have a pharmacist profile. 
            // We use a fallback Pharmacist ID for the record, but the audit log will show the doctor.
            int pharmId = pharmacist?.PharmacistId ?? (await _context.Pharmacists.Select(p => p.PharmacistId).FirstOrDefaultAsync());

            if (pharmId == 0)
            {
                ModelState.AddModelError(string.Empty, "No authorized dispensing unit found in the system.");
                Prescription = prescription;
                return Page();
            }

            var role = User.IsInRole("Doctor") ? "Doctor" : "Pharmacist";

            try
            {
                foreach (var item in prescription.Items)
                {
                    bool success = await _dispensingService.DispenseMedicationAsync(
                        item.PrescriptionItemId, 
                        pharmId, 
                        (double)item.Quantity, 
                        userIdClaim,
                        role
                    );

                    if (!success)
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to dispense {item.Medicine?.MedicineName ?? "Unknown Medication"}. Please check stock availability.");
                        Prescription = prescription;
                        return Page();
                    }
                }

                return RedirectToPage("./Queue", new { success = true, message = "Prescription dispensed successfully." });
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error during dispensing: {ex.Message}");
                Prescription = prescription;
                return Page();
            }
        }
    }
}
