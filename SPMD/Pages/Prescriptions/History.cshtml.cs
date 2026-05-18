using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Prescriptions
{
    [Authorize(Roles = "Doctor,Admin")]
    public class HistoryModel : PageModel
    {
        private readonly AppDbContext _context;

        public HistoryModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Prescription> Prescriptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                var query = _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Items)
                        .ThenInclude(i => i.Medicine)
                    .OrderByDescending(p => p.IssuedAt);

                if (User.IsInRole("Doctor"))
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                    if (doctor != null)
                    {
                        Prescriptions = await query.Where(p => p.DoctorId == doctor.DoctorId).ToListAsync();
                    }
                }
                else
                {
                    Prescriptions = await query.ToListAsync();
                }
            }
        }
    }
}