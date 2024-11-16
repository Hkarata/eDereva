using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string nin, CancellationToken cancellationToken);
        Task<PaginatedResult<UserDto>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken);
        Task<bool> AddAsync(User user, CancellationToken cancellationToken);
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(string nin, CancellationToken cancellationToken);
        Task<List<User>> GetUsersByRoleAsync(Guid roleId, PaginationParams pagination, CancellationToken cancellationToken);
        Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, PaginationParams pagination, CancellationToken cancellationToken);
    }
}
