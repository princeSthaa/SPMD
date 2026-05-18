using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Patients
{
    [Authorize(Roles = "Doctor,Admin")]
    public class ClinicalNotesModel : PageModel
    {
        private readonly AppDbContext _context;

        public ClinicalNotesModel(AppDbContext context)
        {
            _context = context;
        }

        public SPMD.Models.Patient? Patient { get; set; }
        public List<Prescription> PrescriptionHistory { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Patient = await _context.Patients
                .Include(p => p.AllergyRecords)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (Patient == null) return NotFound();

            PrescriptionHistory = await _context.Prescriptions
                .Include(p => p.Items)
                    .ThenInclude(i => i.Medicine)
                .Include(p => p.Doctor)
                .Where(p => p.PatientId == id)
                .OrderByDescending(p => p.IssuedAt)
                .ToListAsync();

            return Page();
        }
    }
}