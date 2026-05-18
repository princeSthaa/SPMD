using Microsoft.EntityFrameworkCore;
using System;

namespace SPMD.Models
{
    public class MedicineBatch
    {
        public int MedicineBatchId { get; set; }
        
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;

        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int QuantityAvailable { get; set; }
        
        [Precision(18, 2)]
        public decimal UnitCost { get; set; } = 0m;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
