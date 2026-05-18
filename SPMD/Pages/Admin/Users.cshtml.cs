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

namespace SPMD.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin,Admin,Pharmacist")]
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public UsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == userId.ToString())
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                if (user.Role.Name == RoleName.SuperAdmin)
                {
                    TempData["ErrorMessage"] = "Super Admin account cannot be deleted.";
                    return RedirectToPage();
                }

                // If the user has a doctor profile, remove it too
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor != null) _context.Doctors.Remove(doctor);

                var pharmacist = await _context.Pharmacists.FirstOrDefaultAsync(ph => ph.UserId == userId);
                if (pharmacist != null) _context.Pharmacists.Remove(pharmacist);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User {user.Username} deleted successfully.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == userId.ToString())
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own account.";
                return RedirectToPage();
            }

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                if (user.Role.Name == RoleName.SuperAdmin && user.IsActive)
                {
                    TempData["ErrorMessage"] = "Super Admin account cannot be deactivated.";
                    return RedirectToPage();
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User {user.Username} {(user.IsActive ? "activated" : "deactivated")} successfully.";
            }
            return RedirectToPage();
        }
    }
}