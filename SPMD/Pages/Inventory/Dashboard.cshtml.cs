using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Inventory
{
    [Authorize(Roles = "Pharmacist,Admin,Doctor")]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Medicine> Medicines { get; set; } = new();
        public List<Medicine> AllMedicines { get; set; } = new();
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;
            AllMedicines = await _context.Medicines.OrderBy(m => m.MedicineName).ToListAsync();

            var query = _context.Medicines
                .Include(m => m.Batches)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m => m.MedicineName.Contains(searchTerm) || 
                                       (m.GenericName != null && m.GenericName.Contains(searchTerm)) || 
                                       (m.Brand != null && m.Brand.Contains(searchTerm)));
            }

            Medicines = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddBatchAsync(int medicineId, string batchNumber, int quantity, DateTime expiryDate)
        {
            var batch = new MedicineBatch
            {
                MedicineId = medicineId,
                BatchNumber = batchNumber,
                QuantityAvailable = quantity,
                ExpiryDate = expiryDate,
                ReceivedAt = DateTime.UtcNow
            };

            _context.MedicineBatches.Add(batch);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRetireBatchAsync(int batchId)
        {
            var batch = await _context.MedicineBatches.FindAsync(batchId);
            if (batch != null)
            {
                batch.QuantityAvailable = 0; // "Retire" the batch by setting stock to zero
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRequestMedicineAsync(int medicineId, int quantity, string clinicalReason)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor != null)
                {
                    var request = new MedicineRequest
                    {
                        MedicineId = medicineId,
                        DoctorId = doctor.DoctorId,
                        RequestedQuantity = quantity,
                        ClinicalReason = clinicalReason,
                        Status = RequestStatus.Pending,
                        RequestedAt = DateTime.UtcNow
                    };

                    _context.MedicineRequests.Add(request);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }
}

