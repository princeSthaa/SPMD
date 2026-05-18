using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Data.Repositories;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Services
{
    public class MedicineService : BaseService<Medicine>
    {
        private readonly AppDbContext _context;

        public MedicineService(IRepository<Medicine> repository, AppDbContext context) : base(repository)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicineAvailabilityDto>> SearchMedicinesAsync(string searchTerm)
        {
            var medicines = await _context.Medicines
                .Include(m => m.Batches)
                .Where(m => string.IsNullOrEmpty(searchTerm) || 
                            m.MedicineName.Contains(searchTerm) || 
                            (m.Brand != null && m.Brand.Contains(searchTerm)) ||
                            (m.GenericName != null && m.GenericName.Contains(searchTerm)))
                .ToListAsync();

            return medicines.Select(m =>
            {
                // Filter out expired batches and sum quantities
                var totalQuantity = m.Batches
                    .Where(b => b.ExpiryDate > DateTime.UtcNow)
                    .Sum(b => b.QuantityAvailable);

                string status = "Out of Stock";
                if (totalQuantity > m.StockThreshold)
                {
                    status = "In Stock";
                }
                else if (totalQuantity > 0)
                {
                    status = "Limited Stock";
                }

                return new MedicineAvailabilityDto
                {
                    MedicineId = m.MedicineId,
                    MedicineName = m.MedicineName,
                    Brand = m.Brand,
                    GenericName = m.GenericName ?? string.Empty,
                    TotalQuantity = totalQuantity,
                    AvailabilityStatus = status,
                    PricePerUnit = m.PricePerUnit
                };
            });
        }

        public async Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync()
        {
            return await _context.Medicines
                .Include(m => m.Batches)
                .Where(m => m.Batches.Where(b => b.ExpiryDate > DateTime.UtcNow).Sum(b => b.QuantityAvailable) <= m.StockThreshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicineBatch>> GetBatchesForMedicineAsync(int medicineId)
        {
            return await _context.MedicineBatches
                .Where(mb => mb.MedicineId == medicineId && mb.QuantityAvailable > 0)
                .OrderBy(mb => mb.ExpiryDate) // FEFO: First Expired First Out
                .ToListAsync();
        }
    }
}
