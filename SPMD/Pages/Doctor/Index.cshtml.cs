using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Doctor
{
    [Authorize(Roles = "SuperAdmin,Doctor")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalPatients { get; set; }
        public int TotalPrescriptions { get; set; }
        public int RecentValidations { get; set; }
        public List<Prescription> RecentPrescriptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor != null)
                {
                    TotalPatients = await _context.Patients.CountAsync();
                    TotalPrescriptions = await _context.Prescriptions.CountAsync(p => p.DoctorId == doctor.DoctorId);
                    RecentValidations = await _context.AuditLogs.CountAsync(a => a.Action == AuditAction.Validate && a.Timestamp >= DateTime.UtcNow.AddDays(-30));

                    RecentPrescriptions = await _context.Prescriptions
                        .Include(p => p.Patient)
                        .Include(p => p.Items)
                        .Where(p => p.DoctorId == doctor.DoctorId)
                        .OrderByDescending(p => p.IssuedAt)
                        .Take(10)
                        .ToListAsync();
                }
            }
        }
    }
}
