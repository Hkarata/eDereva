using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string Nin);
        Task<List<User>> GetAllAsync(PaginationParams pagination);
        Task<bool> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(string Nin);
        Task<List<User>> GetUsersByRoleAsync(Guid roleId, PaginationParams pagination);
        Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, PaginationParams pagination);
    }
}
