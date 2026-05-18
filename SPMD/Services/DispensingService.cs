using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Data.Repositories;
using SPMD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using SPMD.Hubs;

namespace SPMD.Services
{
    public class DispensingService : BaseService<DispensingRecord>
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public DispensingService(IRepository<DispensingRecord> repository, AppDbContext context, AuditService auditService, IHubContext<NotificationHub> hubContext) 
            : base(repository)
        {
            _context = context;
            _auditService = auditService;
            _hubContext = hubContext;
        }

        public async Task<bool> DispenseMedicationAsync(int prescriptionItemId, int pharmacistId, double quantityToDispense, string userId, string role = "Pharmacist")
        {
            // Only start a transaction if one isn't already active
            bool isOuterTransaction = _context.Database.CurrentTransaction == null;
            using var transaction = isOuterTransaction ? await _context.Database.BeginTransactionAsync() : null;

            try
            {
                // 1. Fetch Prescription Item with Medicine and Batches
                var item = await _context.PrescriptionItems
                    .Include(pi => pi.Medicine)
                    .ThenInclude(m => m.Batches)
                    .Include(pi => pi.Prescription)
                    .ThenInclude(p => p.Items) // Need all items to check overall status
                    .FirstOrDefaultAsync(pi => pi.PrescriptionItemId == prescriptionItemId);

                if (item == null || (item.Prescription.Status != PrescriptionStatus.Issued && item.Prescription.Status != PrescriptionStatus.Dispensed))
                {
                    // If it's already dispensed, we might be in a multi-item loop, so we continue but don't error
                    if (item?.Prescription.Status == PrescriptionStatus.Dispensed) return true;
                    return false;
                }

                // 2. Select Batches using FEFO (First Expired First Out)
                var availableBatches = item.Medicine.Batches
                    .Where(b => b.ExpiryDate > DateTime.UtcNow && b.QuantityAvailable > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .ToList();

                double remainingToDispense = quantityToDispense;
                
                if (availableBatches.Sum(b => b.QuantityAvailable) < quantityToDispense)
                {
                    throw new Exception($"Insufficient stock for {item.Medicine.MedicineName}. Available: {availableBatches.Sum(b => b.QuantityAvailable)}");
                }

                foreach (var batch in availableBatches)
                {
                    if (remainingToDispense <= 0) break;

                    int quantityFromBatch = (int)Math.Min((double)batch.QuantityAvailable, remainingToDispense);
                    if (quantityFromBatch <= 0) continue; // Skip if we can't even take one unit

                    // Create Dispensing Record for this batch
                    var record = new DispensingRecord
                    {
                        PrescriptionItemId = prescriptionItemId,
                        PharmacistId = pharmacistId,
                        PatientId = item.Prescription.PatientId,
                        QuantityDispensed = quantityFromBatch,
                        batchNumber = batch.BatchNumber,
                        ExpiryDate = batch.ExpiryDate,
                        DispensedAt = DateTime.UtcNow,
                        IsPartial = quantityToDispense < (double)item.Quantity
                    };

                    // Deduct stock
                    batch.QuantityAvailable -= quantityFromBatch;
                    remainingToDispense -= quantityFromBatch;

                    await _context.DispensingRecords.AddAsync(record);
                }

                // 3. Update Prescription status
                // Mark as dispensed. In a real system, you might check if all items are fulfilled.
                item.Prescription.Status = PrescriptionStatus.Dispensed;

                await _context.SaveChangesAsync();

                // 4. Audit Log
                decimal totalPrice = (decimal)quantityToDispense * item.Medicine.PricePerUnit;
                await _auditService.LogActionAsync(
                    userId, 
                    role, 
                    AuditAction.Dispense, 
                    "Medicine", 
                    item.MedicineId.ToString(), 
                    $"Dispensed {quantityToDispense} units of {item.Medicine.MedicineName} @ {item.Medicine.PricePerUnit:C2}/unit. Total Price: {totalPrice:C2}."
                );

                // 5. Low Stock Check
                var totalStock = item.Medicine.Batches
                    .Where(b => b.ExpiryDate > DateTime.UtcNow)
                    .Sum(b => b.QuantityAvailable);

                if (totalStock <= item.Medicine.StockThreshold)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "System", $"Low Stock Alert: {item.Medicine.MedicineName} only has {totalStock} units remaining (Threshold: {item.Medicine.StockThreshold}).");
                }

                if (transaction != null) await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                if (transaction != null) await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DirectDispenseAsync(int patientId, int doctorId, int pharmacistId, List<PrescriptionItem> items, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var prescription = new Prescription
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    PrescriptionNumber = "DIRECT-" + Guid.NewGuid().ToString().ToUpper().Substring(0, 6),
                    IssuedAt = DateTime.UtcNow,
                    Status = PrescriptionStatus.Issued, // Temp status to allow DispenseMedicationAsync to work
                    Notes = "[DIRECT CLINICAL DISPENSING]"
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.PrescriptionId = prescription.PrescriptionId;
                    _context.PrescriptionItems.Add(item);
                    await _context.SaveChangesAsync();

                    // Process each item using the shared FEFO logic
                    await DispenseMedicationAsync(item.PrescriptionItemId, pharmacistId, (double)item.Quantity, userId);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
