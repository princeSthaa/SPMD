using SPMD.Models;

namespace SPMD.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public string PrescriptionNumber { get; set; } = Guid.NewGuid().ToString();
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ValidUntil { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Issued;
        public string? Notes { get; set; }

        public List<PrescriptionItem> Items { get; set; } = new();
    }
}
