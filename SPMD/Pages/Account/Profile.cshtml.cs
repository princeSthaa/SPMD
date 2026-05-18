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
        public string? FirstName { get; set; }

        [BindProperty]
        public string? LastName { get; set; }

        [BindProperty]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        public DateTime? BirthDate { get; set; }

        [BindProperty]
        public string? HospitalEmployeeNumber { get; set; }

        [BindProperty]
        public string? HealthId { get; set; }

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

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            await LoadUserAsync();
            if (CurrentUser == null) return Page();

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "First Name and Last Name are required.";
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(NewEmail)) CurrentUser.Email = NewEmail;

            _context.Users.Update(CurrentUser);

            if (CurrentUser.Role.Name == RoleName.Doctor && CurrentUser.DoctorProfile != null)
            {
                CurrentUser.DoctorProfile.FirstName = FirstName;
                CurrentUser.DoctorProfile.LastName = LastName;
                if (BirthDate.HasValue) CurrentUser.DoctorProfile.BirthDate = BirthDate.Value;
                CurrentUser.DoctorProfile.Contact = PhoneNumber;
                CurrentUser.DoctorProfile.HospitalEmployeeNumber = HospitalEmployeeNumber;
                _context.Doctors.Update(CurrentUser.DoctorProfile);
            }
            else if (CurrentUser.Role.Name == RoleName.Pharmacist && CurrentUser.PharmacistProfile != null)
            {
                CurrentUser.PharmacistProfile.FirstName = FirstName;
                CurrentUser.PharmacistProfile.LastName = LastName;
                if (BirthDate.HasValue) CurrentUser.PharmacistProfile.BirthDate = BirthDate.Value;
                CurrentUser.PharmacistProfile.Phone = PhoneNumber;
                CurrentUser.PharmacistProfile.HospitalEmployeeNumber = HospitalEmployeeNumber;
                _context.Pharmacists.Update(CurrentUser.PharmacistProfile);
            }
            else if (CurrentUser.Role.Name == RoleName.Patient && CurrentUser.PatientProfile != null)
            {
                CurrentUser.PatientProfile.FirstName = FirstName;
                CurrentUser.PatientProfile.LastName = LastName;
                if (BirthDate.HasValue) CurrentUser.PatientProfile.BirthDate = BirthDate.Value;
                CurrentUser.PatientProfile.Phone = PhoneNumber ?? "";
                CurrentUser.PatientProfile.HealthId = HealthId;
                _context.Patients.Update(CurrentUser.PatientProfile);
            }

            await _context.SaveChangesAsync();
            SuccessMessage = "Profile updated successfully.";
            return Page();
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
                    .Include(u => u.DoctorProfile)
                    .Include(u => u.PharmacistProfile)
                    .Include(u => u.PatientProfile)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                
                if (CurrentUser != null)
                {
                    if (string.IsNullOrEmpty(NewEmail)) NewEmail = CurrentUser.Email;
                    
                    if (string.IsNullOrEmpty(FirstName))
                    {
                        if (CurrentUser.Role.Name == RoleName.Doctor && CurrentUser.DoctorProfile != null)
                        {
                            FirstName = CurrentUser.DoctorProfile.FirstName;
                            LastName = CurrentUser.DoctorProfile.LastName;
                            PhoneNumber = CurrentUser.DoctorProfile.Contact;
                            BirthDate = CurrentUser.DoctorProfile.BirthDate;
                            HospitalEmployeeNumber = CurrentUser.DoctorProfile.HospitalEmployeeNumber;
                        }
                        else if (CurrentUser.Role.Name == RoleName.Pharmacist && CurrentUser.PharmacistProfile != null)
                        {
                            FirstName = CurrentUser.PharmacistProfile.FirstName;
                            LastName = CurrentUser.PharmacistProfile.LastName;
                            PhoneNumber = CurrentUser.PharmacistProfile.Phone;
                            BirthDate = CurrentUser.PharmacistProfile.BirthDate;
                            HospitalEmployeeNumber = CurrentUser.PharmacistProfile.HospitalEmployeeNumber;
                        }
                        else if (CurrentUser.Role.Name == RoleName.Patient && CurrentUser.PatientProfile != null)
                        {
                            FirstName = CurrentUser.PatientProfile.FirstName;
                            LastName = CurrentUser.PatientProfile.LastName;
                            PhoneNumber = CurrentUser.PatientProfile.Phone;
                            BirthDate = CurrentUser.PatientProfile.BirthDate;
                            HealthId = CurrentUser.PatientProfile.HealthId;
                        }
                    }
                }
            }
        }
    }
}
