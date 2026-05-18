using SPMD.Models;

namespace SPMD.Models
{
  public class Role
  {
    public int RoleId { get; set; }
    public RoleName Name { get; set; }

    public List<User> Users { get; set; } = new();
  }
}
