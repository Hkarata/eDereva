using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class RoleRepository(ApplicationDbContext context, ILogger<RoleRepository> logger) : IRoleRepository
    {
        public async Task<bool> AddAsync(Role role, CancellationToken cancellationToken)
        {
            logger.LogInformation("Checking if the role with name {RoleName} already exists.", role.Name);

            var roleExists = await CheckRoleNameExistsAsync(role.Name, cancellationToken);

            if (roleExists)
            {
                logger.LogWarning("Role with name {RoleName} already exists. Skipping add operation.", role.Name);
                return false;
            }

            logger.LogInformation("Adding role with name {RoleName}.", role.Name);
            await context.Roles.AddAsync(role, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Role with name {RoleName} added successfully.", role.Name);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid roleId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to delete role with ID {RoleId}.", roleId);

            var role = await context.Roles.FindAsync(roleId);
            if (role == null)
            {
                logger.LogWarning("Role with ID {RoleId} not found. Delete operation skipped.", roleId);
                return false;
            }

            context.Roles.Remove(role);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Role with ID {RoleId} deleted successfully.", roleId);
            return true;
        }

        public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching all roles from the database.");

            var roles = await context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Fetched {RoleCount} roles from the database.", roles.Count);
            return roles;
        }

        public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching role with ID {RoleId} and its permissions.", roleId);

            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken: cancellationToken);

            if (role == null)
            {
                logger.LogWarning("Role with ID {RoleId} not found or has no permissions.", roleId);
            }

            return role;
        }

        public async Task<List<Guid>> GetRoleIdsAsync(CancellationToken cancellationToken)
        {
            var roleIds = await context.Roles
                .AsNoTracking()
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            return roleIds;
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching role with ID {RoleName} and its permissions.", roleName);

            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.Permission)
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower(), cancellationToken: cancellationToken);

            if (role == null)
            {
                logger.LogWarning("Role with ID {RoleId} not found or has no permissions.", role!.Id);
            }

            return role;
        }

        public async Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating role with ID {RoleId}.", role.Id);

            var entry = context.Entry(role);
            if (entry.State == EntityState.Detached)
            {
                context.Roles.Attach(role);
            }

            entry.State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Role with ID {RoleId} updated successfully.", role.Id);
            return role;
        }

        private async Task<bool> CheckRoleNameExistsAsync(string roleName, CancellationToken cancellationToken)
        {
            logger.LogInformation("Checking if the role with name {RoleName} exists in the database.", roleName);

            var exists = await context.Roles
                .AnyAsync(r => r.Name.ToLower() == roleName.ToLower(), cancellationToken);

            logger.LogInformation("Role with name {RoleName} exists: {Exists}.", roleName, exists);
            return exists;
        }

    }
}