using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SPMD.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Doctor")) return RedirectToPage("/Doctor/Index");
            if (User.IsInRole("Pharmacist")) return RedirectToPage("/Pharmacist/Index");
            if (User.IsInRole("Patient")) return RedirectToPage("/Patient/Index");
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")) return RedirectToPage("/Admin/Users");
        }
        return Page();
    }
}
