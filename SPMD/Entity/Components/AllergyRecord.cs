using SPMD.Models;

namespace SPMD.Models
{
  public class AllergyRecord
  {
    public int AllergyRecordId { get; set; }
    public string? Substance { get; set; }           // free-text for non-medicine allergies
    public string? Reaction { get; set; }
    public SeverityLevel Severity { get; set; } = SeverityLevel.Moderate;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? MedicineId { get; set; }
    public Medicine? Medicine { get; set; }
  }
}
