using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<PaginatedResult<T>> GetAllAsync(PaginationParams pagination);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(object id);
    }
}
