using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System;
using System.Threading.Tasks;

namespace SPMD.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(string userId, string role, AuditAction action, string entityName, string entityId, string? details = null)
        {
            var log = new AuditLog
            {
                PerformedByUserId = userId,
                PerformedByRole = role,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
