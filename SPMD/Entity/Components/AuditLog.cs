namespace SPMD.Models
{
  public class AuditLog
  {
    public int AuditLogId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string PerformedByUserId { get; set; } = string.Empty;
    public string PerformedByRole { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
  }
}
