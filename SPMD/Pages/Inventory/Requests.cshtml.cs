using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Inventory
{
    [Authorize(Roles = "Pharmacist,Admin")]
    public class RequestsModel : PageModel
    {
        private readonly AppDbContext _context;

        public RequestsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<MedicineRequest> PendingRequests { get; set; } = new();
        public List<MedicineRequest> PastRequests { get; set; } = new();

        public async Task OnGetAsync()
        {
            var requests = await _context.MedicineRequests
                .Include(r => r.Medicine)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            PendingRequests = requests.Where(r => r.Status == RequestStatus.Pending).ToList();
            PastRequests = requests.Where(r => r.Status != RequestStatus.Pending).Take(20).ToList();
        }

        public async Task<IActionResult> OnPostFulfillAsync(int requestId)
        {
            var request = await _context.MedicineRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = RequestStatus.Fulfilled;
                request.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int requestId)
        {
            var request = await _context.MedicineRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = RequestStatus.Cancelled;
                request.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}