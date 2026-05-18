using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Dispensing
{
    [Authorize(Roles = "Pharmacist,Admin,Doctor")]
    public class QueueModel : PageModel

    {
        private readonly AppDbContext _context;

        public QueueModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Prescription> PendingPrescriptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            PendingPrescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                .Where(p => p.Status == PrescriptionStatus.Issued)
                .OrderByDescending(p => p.IssuedAt)
                .ToListAsync();
        }
    }
}
