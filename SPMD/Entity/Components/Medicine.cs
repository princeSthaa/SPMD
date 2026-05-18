using Microsoft.EntityFrameworkCore;
using SPMD.Models;

namespace SPMD.Models
{
  public class Medicine
  {
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? GenericName { get; set; }
    public string? Strength { get; set; }
    public string? DosageForm { get; set; }
    
    [Precision(18, 2)]
    public decimal PricePerUnit { get; set; } = 0m;

    public bool IsControlled { get; set; } = false;
    public int StockThreshold { get; set; } = 10; // Default threshold for low stock alerts
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<MedicineBatch> Batches { get; set; } = new();
    public List<PrescriptionItem> PrescriptionItems { get; set; } = new();
    public List<AllergyRecord> AllergyRecords { get; set; } = new();
  }
}
