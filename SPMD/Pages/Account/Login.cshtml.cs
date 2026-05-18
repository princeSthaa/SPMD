using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPMD.Models;
using SPMD.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public LoginModel(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [BindProperty]
        public LoginDto Input { get; set; } = new();

        public string? ReturnUrl { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet(string? returnUrl = null, string? success = null)
        {
            ReturnUrl = returnUrl;
            SuccessMessage = success;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.GetByUsernameAsync(Input.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact the administrator.");
                return Page();
            }

            // Enforce Role Restriction (Bypass for SuperAdmin)
            if (user.Role.Name != RoleName.SuperAdmin)
            {
                // Ensure the user's role matches the selected login role
                if ((int)user.Role.Name != Input.RoleId)
                {
                    var roleName = Input.RoleId switch
                    {
                        1 => "Pharmacist",
                        2 => "Doctor",
                        4 => "Patient",
                        _ => "selected role"
                    };
                    ModelState.AddModelError(string.Empty, $"No user found under name '{Input.Username}' for the role of {roleName}.");
                    return Page();
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.RequiresPasswordChange)
            {
                return RedirectToPage("/Account/SetupCredentials");
            }

            return LocalRedirect(returnUrl ?? "/Index");
        }
    }
}
