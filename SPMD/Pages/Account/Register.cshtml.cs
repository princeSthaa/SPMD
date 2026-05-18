using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPMD.Models;
using SPMD.Services;
using System.Threading.Tasks;

namespace SPMD.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AuthService _authService;

        public RegisterModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public RegisterDto Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _authService.RegisterAsync(Input);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists or registration failed.");
                return Page();
            }

            return RedirectToPage("./Login");
        }
    }
}
