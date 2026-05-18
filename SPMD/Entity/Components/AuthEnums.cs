namespace SPMD.Models
{
  public enum SeverityLevel { Low = 0, Moderate = 1, Severe = 2 }
  public enum PrescriptionStatus { Draft = 0, Issued = 1, Cancelled = 2, Dispensed = 3 }
  public enum AuditAction { Create = 0, Update = 1, Delete = 2, Dispense = 3, Validate = 4 }
  public enum RoleName { Admin = 0, Pharmacist = 1, Doctor = 2, Receptionist = 3, Patient = 4, SuperAdmin = 5 }
}

