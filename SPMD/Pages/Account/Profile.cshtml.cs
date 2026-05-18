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
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProfileModel(AppDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser { get; set; }

        [BindProperty]
        public string? NewEmail { get; set; }

        [BindProperty]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadUserAsync();
        }

        public async Task<IActionResult> OnPostUpdateEmailAsync()
        {
            await LoadUserAsync();
            if (CurrentUser == null || string.IsNullOrWhiteSpace(NewEmail))
            {
                ErrorMessage = "Invalid email update attempt.";
                return Page();
            }

            CurrentUser.Email = NewEmail;
            _context.Users.Update(CurrentUser);
            await _context.SaveChangesAsync();

            SuccessMessage = "Email updated successfully.";
            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePasswordAsync()
        {
            await LoadUserAsync();
            if (CurrentUser == null || string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "Please fill in all password fields.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New passwords do not match.";
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, CurrentUser.PasswordHash))
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            CurrentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            _context.Users.Update(CurrentUser);
            await _context.SaveChangesAsync();

            SuccessMessage = "Password updated successfully.";
            return Page();
        }

        private async Task LoadUserAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                CurrentUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                
                if (CurrentUser != null && string.IsNullOrEmpty(NewEmail))
                {
                    NewEmail = CurrentUser.Email;
                }
            }
        }
    }
}
