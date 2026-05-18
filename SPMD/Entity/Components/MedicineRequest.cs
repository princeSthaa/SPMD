using SPMD.Models;
using System;

namespace SPMD.Models
{
    public enum RequestStatus { Pending = 0, Fulfilled = 1, Cancelled = 2 }

    public class MedicineRequest
    {
        public int MedicineRequestId { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int RequestedQuantity { get; set; }
        public string? ClinicalReason { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}