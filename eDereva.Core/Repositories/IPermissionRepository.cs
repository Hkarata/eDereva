using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface IPermissionRepository
{
    Task<Permission> GetByIdAsync(int permissionId);
    Task<List<Permission>> GetAllAsync();
    Task<Permission> AddAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task DeleteAsync(Permission permission);
}