using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using SPMD.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Patients
{
    [Authorize(Roles = "Doctor,Pharmacist,Admin")]
    public class SearchModel : PageModel
    {
        private readonly AppDbContext _context;

        public SearchModel(AppDbContext context)
        {
            _context = context;
        }

        public string? SearchTerm { get; set; }
        public IEnumerable<SPMD.Models.Patient>? Patients { get; set; }

        [BindProperty]
        public RegisterDto NewPatient { get; set; } = new();

        public string? Message { get; set; }

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;
            Patients = await GetPatientsAsync(searchTerm);
        }

        public async Task<IActionResult> OnPostRegisterPatientAsync()
        {
            // Force Role to Patient
            NewPatient.RoleId = (int)RoleName.Patient;
            
            var authService = HttpContext.RequestServices.GetRequiredService<AuthService>();
            var user = await authService.RegisterAsync(NewPatient);

            if (user != null)
            {
                Message = "Patient registered successfully.";
                return RedirectToPage(new { searchTerm = NewPatient.Username });
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Username might already exist.");
            Patients = await GetPatientsAsync(null);
            return Page();
        }

        public async Task<PartialViewResult> OnGetSearchPartialAsync(string? searchTerm)
        {
            var results = await GetPatientsAsync(searchTerm);
            return Partial("_PatientSearchResults", results);
        }

        private async Task<List<SPMD.Models.Patient>> GetPatientsAsync(string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return await _context.Patients
                    .Include(p => p.AllergyRecords)
                    .Where(p => p.FirstName.Contains(searchTerm) || 
                                p.LastName.Contains(searchTerm) || 
                                p.Email.Contains(searchTerm) || 
                                p.Phone.Contains(searchTerm))
                    .ToListAsync();
            }
            else
            {
                return await _context.Patients
                    .Include(p => p.AllergyRecords)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .ToListAsync();
            }
        }
    }
}
