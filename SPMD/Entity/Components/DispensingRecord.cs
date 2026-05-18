using SPMD.Models;


namespace SPMD.Models
{
  public class DispensingRecord
  {
    public int DispensingRecordId { get; set; }
    public DateTime DispensedAt { get; set; } = DateTime.UtcNow;
    public double QuantityDispensed { get; set; } = 0.0;
    public string batchNumber { get; set; } = String.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsPartial { get; set; }

    public int PrescriptionItemId { get; set; }
    public PrescriptionItem? PrescriptionItem { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int PharmacistId { get; set; }
    public Pharmacist? Pharmacist { get; set; }
  }
}
