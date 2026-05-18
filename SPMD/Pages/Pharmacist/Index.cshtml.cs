using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using SPMD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Pharmacist
{
    [Authorize(Roles = "SuperAdmin,Pharmacist")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly MedicineService _medicineService;

        public IndexModel(AppDbContext context, MedicineService medicineService)
        {
            _context = context;
            _medicineService = medicineService;
        }

        public List<Medicine> LowStockMedicines { get; set; } = new();
        public List<MedicineBatch> ExpiringBatches { get; set; } = new();
        public List<DispensingRecord> RecentDispensing { get; set; } = new();

        public async Task OnGetAsync()
        {
            LowStockMedicines = (await _medicineService.GetLowStockMedicinesAsync()).ToList();

            ExpiringBatches = await _context.MedicineBatches
                .Include(mb => mb.Medicine)
                .Where(mb => mb.ExpiryDate <= DateTime.UtcNow.AddDays(30) && mb.ExpiryDate > DateTime.UtcNow && mb.QuantityAvailable > 0)
                .OrderBy(mb => mb.ExpiryDate)
                .Take(5)
                .ToListAsync();

            RecentDispensing = await _context.DispensingRecords
                .Include(dr => dr.Patient)
                .Include(dr => dr.Pharmacist)
                .Include(dr => dr.PrescriptionItem)
                .ThenInclude(pi => pi.Medicine)
                .OrderByDescending(dr => dr.DispensedAt)
                .Take(10)
                .ToListAsync();
        }
    }
}
