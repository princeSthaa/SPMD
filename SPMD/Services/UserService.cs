using SPMD.Models;
using SPMD.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SPMD.Data;

namespace SPMD.Services
{
  public class UserService : BaseService<User>
  {
    private readonly AppDbContext _context;

    public UserService(IRepository<User> repository, AppDbContext context) : base(repository)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }
  }
}
