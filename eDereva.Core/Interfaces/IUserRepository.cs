using eDereva.Core.Entities;

namespace eDereva.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task<List<User>> GetAllAsync(ISpecification<User>? specification = null!);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);

        // Example of finding users by their roles or permissions
        Task<List<User>> GetUsersByRoleAsync(Guid roleId, ISpecification<User>? specification = null!);
        Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, ISpecification<User>? specification = null!);
    }
}
