using SPMD.Models;

namespace SPMD.Models
{
  public class Doctor
  {
    public int DoctorId { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string? HospitalEmployeeNumber { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<Prescription> Prescriptions { get; set; } = new();
  }
}

