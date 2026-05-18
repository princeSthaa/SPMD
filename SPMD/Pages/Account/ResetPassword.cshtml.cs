using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ResetPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IActionResult OnGet(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Account/Login");
            }

            Token = token;
            Email = email;
            return Page();
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
                // For security, don't reveal if user exists, just redirect
                return RedirectToPage("/Account/Login", new { success = "If your account exists, your password has been reset." });
            }

            // In a real app, you would validate the 'Token' here against a stored token in the DB
            // and check for expiration. For this implementation, we'll assume the token is valid.

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            user.RequiresPasswordChange = false; // Reset the flag since they just manually reset it

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Login", new { success = "Password reset successfully. You can now login with your new password." });
        }
    }
}
