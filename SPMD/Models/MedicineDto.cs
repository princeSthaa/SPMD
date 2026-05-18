namespace SPMD.Models
{
    public class MedicineAvailabilityDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string GenericName { get; set; } = string.Empty;
        public string AvailabilityStatus { get; set; } = string.Empty; // "In Stock", "Limited Stock", "Out of Stock"
        public int TotalQuantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
