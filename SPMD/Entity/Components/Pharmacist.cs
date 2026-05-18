using SPMD.Models;

namespace SPMD.Models
{
  public class Pharmacist
  {
    public int PharmacistId { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<DispensingRecord> DispensingRecords { get; set; } = new();
    public List<AuditLog> AuditLogs { get; set; } = new();

  }
}
