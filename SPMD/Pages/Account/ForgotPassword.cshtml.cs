using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ForgotPasswordModel(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
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
                // For security, don't reveal if user exists
                Message = "If an account with that email exists, a reset link has been sent.";
                return Page();
            }

            // Generate a simple "token" (in a real app, use a secure token with expiry)
            var resetToken = Guid.NewGuid().ToString();
            // Store this token in the database for the user (optional, but recommended for validation)
            // For now, let's just simulate the email send.
            
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { token = resetToken, email = Email },
                protocol: Request.Scheme);

            await _emailService.SendEmailAsync(
                Email,
                "Reset Password",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

            Message = "If an account with that email exists, a reset link has been sent.";
            return Page();
        }
    }
}
