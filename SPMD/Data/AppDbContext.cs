using Microsoft.EntityFrameworkCore;
using SPMD.Models;
namespace SPMD.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Prescription -> Patient (prevent cascade)
    modelBuilder.Entity<Prescription>()
        .HasOne(p => p.Patient)
        .WithMany(pt => pt.Prescriptions)
        .HasForeignKey(p => p.PatientId)
        .OnDelete(DeleteBehavior.Restrict);

    // Prescription -> Doctor (prevent cascade)
    modelBuilder.Entity<Prescription>()
        .HasOne(p => p.Doctor)
        .WithMany(d => d.Prescriptions)
        .HasForeignKey(p => p.DoctorId)
        .OnDelete(DeleteBehavior.Restrict);

    // PrescriptionItem -> Prescription (prevent cascade)
    modelBuilder.Entity<PrescriptionItem>()
        .HasOne(pi => pi.Prescription)
        .WithMany(p => p.Items)
        .HasForeignKey(pi => pi.PrescriptionId)
        .OnDelete(DeleteBehavior.Restrict);

    // DispensingRecord -> PrescriptionItem (prevent cascade)
    modelBuilder.Entity<DispensingRecord>()
        .HasOne(dr => dr.PrescriptionItem)
        .WithMany(pi => pi.DispensingRecords)
        .HasForeignKey(dr => dr.PrescriptionItemId)
        .OnDelete(DeleteBehavior.Restrict);

    // DispensingRecord -> Patient (prevent cascade)
    modelBuilder.Entity<DispensingRecord>()
        .HasOne(dr => dr.Patient)
        .WithMany(p => p.DispensingRecords)
        .HasForeignKey(dr => dr.PatientId)
        .OnDelete(DeleteBehavior.Restrict);

    // DispensingRecord -> Pharmacist (optional)
    modelBuilder.Entity<DispensingRecord>()
        .HasOne(dr => dr.Pharmacist)
        .WithMany(ph => ph.DispensingRecords)
        .HasForeignKey(dr => dr.PharmacistId)
        .OnDelete(DeleteBehavior.Restrict);

    // MedicineBatch -> Medicine (prevent cascade)
    modelBuilder.Entity<MedicineBatch>()
        .HasOne(mb => mb.Medicine)
        .WithMany(m => m.Batches)
        .HasForeignKey(mb => mb.MedicineId)
        .OnDelete(DeleteBehavior.Restrict);

    // Align table names with existing migrations (singular)
    modelBuilder.Entity<User>().ToTable("User");
    modelBuilder.Entity<Role>().ToTable("Role");
    modelBuilder.Entity<Patient>().ToTable("Patient");
    modelBuilder.Entity<Medicine>().ToTable("Medicine");
    modelBuilder.Entity<AllergyRecord>().ToTable("AllergyRecord");
    modelBuilder.Entity<AuditLog>().ToTable("AuditLog");
    modelBuilder.Entity<DispensingRecord>().ToTable("DispensingRecord");
    modelBuilder.Entity<Pharmacist>().ToTable("Pharmacist");
    modelBuilder.Entity<Prescription>().ToTable("Prescription");
    modelBuilder.Entity<PrescriptionItem>().ToTable("PrescriptionItem");
    modelBuilder.Entity<MedicineBatch>().ToTable("MedicineBatch");
    // Doctors is plural in the InitialCreate migration
    modelBuilder.Entity<Doctor>().ToTable("Doctors");
  }


  public DbSet<Doctor> Doctors { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;
  public DbSet<Patient> Patients { get; set; } = null!;
  public DbSet<Prescription> Prescriptions { get; set; } = null!;
  public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!;
  public DbSet<Medicine> Medicines { get; set; } = null!;
  public DbSet<DispensingRecord> DispensingRecords { get; set; } = null!;
  public DbSet<AllergyRecord> AllergyRecords { get; set; } = null!;
  public DbSet<Role> Roles { get; set; } = null!;
  public DbSet<Pharmacist> Pharmacists { get; set; } = null!;
  public DbSet<AuditLog> AuditLogs { get; set; } = null!;
  public DbSet<MedicineBatch> MedicineBatches { get; set; } = null!;
  public DbSet<MedicineRequest> MedicineRequests { get; set; } = null!;
  public DbSet<DrugInteraction> DrugInteractions { get; set; } = null!;

}
