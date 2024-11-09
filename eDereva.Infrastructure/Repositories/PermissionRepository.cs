using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    internal class PermissionRepository(ApplicationDbContext context) : IPermissionRepository
    {
        public async Task<Permission> GetByIdAsync(int permissionId)
        {
            return await context.Permissions
                                 .FirstOrDefaultAsync(p => p.Id == permissionId) ?? new();
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await context.Permissions.ToListAsync();
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            await context.Permissions.AddAsync(permission);
            await context.SaveChangesAsync();
            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            context.Permissions.Update(permission);
            await context.SaveChangesAsync();
            return permission;
        }

        public async Task DeleteAsync(Permission permission)
        {
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
        }
    }
}
