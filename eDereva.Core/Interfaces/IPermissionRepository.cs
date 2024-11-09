using eDereva.Core.Entities;

namespace eDereva.Core.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission> GetByIdAsync(int permissionId);
        Task<List<Permission>> GetAllAsync();
        Task<Permission> AddAsync(Permission permission);
        Task<Permission> UpdateAsync(Permission permission);
        Task DeleteAsync(Permission permission);
    }
}
