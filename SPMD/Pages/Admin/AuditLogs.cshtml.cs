using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMD.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin,Admin,Pharmacist")]
    public class AuditLogsModel : PageModel
    {
        private readonly AppDbContext _context;

        public AuditLogsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<AuditLog> AuditLogs { get; set; } = new();
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(string? searchTerm, int? actionFilter)
        {
            SearchTerm = searchTerm;
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.PerformedByUserId.Contains(searchTerm) || a.EntityName.Contains(searchTerm));
            }

            if (actionFilter.HasValue)
            {
                query = query.Where(a => (int)a.Action == actionFilter.Value);
            }

            AuditLogs = await query.OrderByDescending(a => a.Timestamp).Take(100).ToListAsync();
        }
    }
}
