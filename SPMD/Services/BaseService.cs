using SPMD.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPMD.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IRepository<T> _repository;

        public BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveAsync();
            return true;
        }
    }
}
