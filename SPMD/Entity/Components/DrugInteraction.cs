namespace SPMD.Models
{
    public class DrugInteraction
    {
        public int DrugInteractionId { get; set; }
        public string GenericNameA { get; set; } = string.Empty;
        public string GenericNameB { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SeverityLevel Severity { get; set; } = SeverityLevel.Moderate;
    }
}
