using eDereva.Core.Entities;
using eDereva.Core.Enums;

namespace eDereva.Core.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> AddAsync(Role role, CancellationToken cancellationToken);
    Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid roleId, CancellationToken cancellationToken);
    Task<List<Guid>> GetRoleIdsAsync(CancellationToken cancellationToken);
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken);
    Task AddBasicUserRole(string userNin, CancellationToken cancellationToken);
    Task<PermissionFlag> GetBasicRolePermissionFlag();
}