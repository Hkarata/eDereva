using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class PermissionRepository(ApplicationDbContext context, HybridCache hybridCache, ILogger<PermissionRepository> logger) : IPermissionRepository
    {
        public async Task<Permission> GetByIdAsync(int permissionId)
        {
            var cacheKey = $"Permission_{permissionId}";
            logger.LogInformation("Fetching permission with ID {PermissionId} from cache or database.", permissionId);

            var permission = await hybridCache.GetOrCreateAsync<Permission>(cacheKey, async (entry) =>
            {
                logger.LogInformation("Permission with ID {PermissionId} not found in cache. Fetching from database.", permissionId);
                return await context.Permissions
                                     .FirstOrDefaultAsync(p => p.Id == permissionId, cancellationToken: entry) ?? new();
            });

            logger.LogInformation("Permission with ID {PermissionId} fetched successfully.", permissionId);
            return permission!;
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            var cacheKey = "Permissions";
            logger.LogInformation("Fetching all permissions from cache or database.");

            var permissions = await hybridCache.GetOrCreateAsync<List<Permission>>(cacheKey, async (entry) =>
            {
                logger.LogInformation("Permissions not found in cache. Fetching from database.");
                return await context.Permissions.ToListAsync(cancellationToken: entry);
            });

            logger.LogInformation("All permissions fetched successfully.");
            return permissions!;
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            logger.LogInformation("Adding new permission to the database.");
            await context.Permissions.AddAsync(permission);
            await context.SaveChangesAsync();
            logger.LogInformation("Permission added successfully with ID {PermissionId}.", permission.Id);
            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            logger.LogInformation("Updating permission with ID {PermissionId}.", permission.Id);
            context.Permissions.Update(permission);
            await context.SaveChangesAsync();
            logger.LogInformation("Permission with ID {PermissionId} updated successfully.", permission.Id);
            return permission;
        }

        public async Task DeleteAsync(Permission permission)
        {
            logger.LogInformation("Deleting permission with ID {PermissionId}.", permission.Id);
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
            logger.LogInformation("Permission with ID {PermissionId} deleted successfully.", permission.Id);
        }
    }
}
