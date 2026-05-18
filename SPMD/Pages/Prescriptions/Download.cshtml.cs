using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Services;
using System.Threading.Tasks;

namespace SPMD.Pages.Prescriptions
{
    [Authorize]
    public class DownloadModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly PdfService _pdfService;

        public DownloadModel(AppDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GeneratePrescriptionPdf(prescription);
            return File(pdfBytes, "application/pdf", $"Prescription_{prescription.PrescriptionNumber}.pdf");
        }
    }
}
