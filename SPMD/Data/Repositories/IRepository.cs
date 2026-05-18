using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPMD.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveAsync();
    }
}
