namespace SPMD.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@(gmail\.com|hotmail\.com)$", ErrorMessage = "Only @gmail.com and @hotmail.com email addresses are allowed.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string? HospitalEmployeeNumber { get; set; }
        
        [RegularExpression(@"^SPMD-\d{3}$", ErrorMessage = "Health ID must be in the format SPMD-123")]
        public string? HealthId { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
