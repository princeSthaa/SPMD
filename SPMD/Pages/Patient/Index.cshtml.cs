using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using SPMD.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Patient
{
    [Authorize(Roles = "SuperAdmin,Patient")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly PrescriptionService _prescriptionService;

        public IndexModel(AppDbContext context, PrescriptionService prescriptionService)
        {
            _context = context;
            _prescriptionService = prescriptionService;
        }

        public SPMD.Models.Patient? PatientInfo { get; set; }
        public IEnumerable<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                PatientInfo = await _context.Patients
                    .Include(p => p.AllergyRecords)
                    .Include(p => p.DispensingRecords)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (PatientInfo != null)
                {
                    Prescriptions = await _prescriptionService.GetPrescriptionsForPatientAsync(PatientInfo.PatientId);
                }
            }
        }
    }
}
