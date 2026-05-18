using SPMD.Models;

namespace SPMD.Models
{
    public class Patient
    {
        public int PatientId { get; set; }   // primary key

        public int? UserId { get; set; }
        public User? User { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? OptionalAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Prescription> Prescriptions { get; set; } = new();
        public List<AllergyRecord> AllergyRecords { get; set; } = new();
        public List<DispensingRecord> DispensingRecords { get; set; } = new();
    }
}
