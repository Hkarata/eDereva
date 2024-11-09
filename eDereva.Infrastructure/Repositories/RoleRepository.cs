using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class RoleRepository(ApplicationDbContext context, HybridCache hybrdiCache, ILogger<RoleRepository> logger) : IRoleRepository
    {

        public async Task<Role> GetByIdAsync(Guid roleId)
        {
            var cacheKey = $"Role_{roleId}";

            logger.LogInformation("Fetching role with ID {RoleId} from cache or database.", roleId);

            var role = await hybrdiCache.GetOrCreateAsync<Role>(cacheKey, async (entry) =>
            {
                return await context.Roles
                    .Where(r => r.Id == roleId)
                    .Include(r => r.Permissions)
                    .FirstOrDefaultAsync(cancellationToken: entry) ?? null!;
            });

            if (role == null)
            {
                logger.LogWarning("Role with ID {RoleId} not found.", roleId);
            }
            else
            {
                logger.LogInformation("Role with ID {RoleId} retrieved successfully.", roleId);
            }

            return role!;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            var cacheKey = "Roles";

            logger.LogInformation("Fetching all roles from cache or database.");

            var roles = await hybrdiCache.GetOrCreateAsync<List<Role>>(cacheKey, async (entry) =>
            {
                return await context.Roles.ToListAsync(cancellationToken: entry);
            });

            logger.LogInformation("{Count} roles retrieved successfully.", roles.Count);

            return roles;
        }

        public async Task<Role> AddAsync(Role role)
        {
            logger.LogInformation("Adding a new role with ID {RoleId}.", role.Id);
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            logger.LogInformation("Role with ID {RoleId} added successfully.", role.Id);
            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            logger.LogInformation("Updating role with ID {RoleId}.", role.Id);
            context.Roles.Update(role);
            await context.SaveChangesAsync();
            logger.LogInformation("Role with ID {RoleId} updated successfully.", role.Id);
            return role;
        }

        public async Task DeleteAsync(Role role)
        {
            logger.LogInformation("Deleting role with ID {RoleId}.", role.Id);
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
            logger.LogInformation("Role with ID {RoleId} deleted successfully.", role.Id);
        }

        public async Task<List<Role>> GetRolesWithPermissionsAsync(List<Guid> roleIds)
        {
            var cacheKey = $"{roleIds.Count} roles";
            logger.LogInformation("Fetching roles with permissions for {Count} role IDs.", roleIds.Count);

            var roles = await hybrdiCache.GetOrCreateAsync<List<Role>>(cacheKey, async (entry) =>
            {
                return await context.Roles
                    .Where(r => roleIds.Contains(r.Id))
                    .Include(r => r.Permissions)
                    .ToListAsync(cancellationToken: entry);
            });

            logger.LogInformation("{Count} roles with permissions retrieved successfully.", roles.Count);
            return roles ?? null!;
        }

        public async Task<List<Guid>> GetRoleIdsAsync()
        {
            var cacheKey = "RoleIds";
            logger.LogInformation("Fetching all role IDs from cache or database.");

            var roleIds = await hybrdiCache.GetOrCreateAsync(cacheKey, async (entry) =>
            {
                return await context.Roles.Select(r => r.Id).ToListAsync(cancellationToken: entry);
            });

            logger.LogInformation("{Count} role IDs retrieved successfully.", roleIds.Count);
            return roleIds;
        }
    }
}
