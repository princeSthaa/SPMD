using SPMD.Models;
using Microsoft.EntityFrameworkCore;

namespace SPMD.Models
{
  public class PrescriptionItem
  {
    public int PrescriptionItemId { get; set; }

    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;

    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;

    [Precision(10, 3)]
    public decimal Quantity { get; set; } = 0m;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; } = 0;
    public int RefillsAllowed { get; set; } = 0;

    public List<DispensingRecord> DispensingRecords { get; set; } = new();
  }
}
