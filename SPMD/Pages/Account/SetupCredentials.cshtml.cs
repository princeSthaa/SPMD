using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    [Authorize]
    public class SetupCredentialsModel : PageModel
    {
        private readonly AppDbContext _context;

        public SetupCredentialsModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string NewUsername { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.RequiresPasswordChange)
            {
                return RedirectToPage("/Index");
            }

            NewUsername = user.Username;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");

            // Check if username is already taken by someone else
            var existingUser = await _context.Users.AnyAsync(u => u.Username == NewUsername && u.UserId != userId);
            if (existingUser)
            {
                ErrorMessage = "Username is already taken.";
                return Page();
            }

            user.Username = NewUsername;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            user.RequiresPasswordChange = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Re-sign in to update the username in the cookie
            await HttpContext.SignOutAsync();
            
            return RedirectToPage("/Account/Login", new { success = "Credentials updated successfully. Please login with your new credentials." });
        }
    }
}
