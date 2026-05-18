using SPMD.Models;

namespace SPMD.Models
{
  public class User
  {
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public bool IsActive { get; set; } = false;
    public bool RequiresPasswordChange { get; set; } = false;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional links to domain entities
    public Doctor? DoctorProfile { get; set; }
    public Pharmacist? PharmacistProfile { get; set; }
    public Patient? PatientProfile { get; set; }
  }
}
