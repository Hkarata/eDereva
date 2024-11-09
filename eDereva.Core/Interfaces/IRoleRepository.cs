using eDereva.Core.Entities;

namespace eDereva.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(Guid roleId);
        Task<List<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<List<Role>> GetRolesWithPermissionsAsync(IEnumerable<Guid> roleIds);
    }
}
