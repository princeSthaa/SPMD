using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Data.Repositories;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Services
{
    public class PatientService : BaseService<Patient>
    {
        private readonly AppDbContext _context;

        public PatientService(IRepository<Patient> repository, AppDbContext context) : base(repository)
        {
            _context = context;
        }

        public async Task<Patient?> GetPatientWithAllergiesAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.AllergyRecords)
                .ThenInclude(a => a.Medicine)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<AllergyRecord> AddAllergyAsync(AllergyRecord allergyRecord)
        {
            await _context.AllergyRecords.AddAsync(allergyRecord);
            await _context.SaveChangesAsync();
            return allergyRecord;
        }
    }
}
