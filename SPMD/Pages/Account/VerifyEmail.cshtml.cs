using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly AppDbContext _context;

        public VerifyEmailModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                ErrorMessage = "Account not found.";
                return Page();
            }

            if (user.IsActive)
            {
                SuccessMessage = "Account is already verified. You can log in.";
                return RedirectToPage("./Login");
            }

            if (user.VerificationCode != Code)
            {
                ErrorMessage = "Invalid verification code.";
                return Page();
            }

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                ErrorMessage = "Verification code has expired. Please contact support.";
                return Page();
            }

            user.IsActive = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Email verified successfully! You can now log in.";
            return RedirectToPage("./Login");
        }
    }
}
