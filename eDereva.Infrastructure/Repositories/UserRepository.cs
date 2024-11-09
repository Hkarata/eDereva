using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context, HybridCache hybridCache, ILogger<UserRepository> logger) : IUserRepository
    {

        public async Task<User> GetByIdAsync(Guid userId)
        {
            var cacheKey = $"User_{userId}";
            logger.LogInformation("Fetching user with ID {UserId} from cache or database.", userId);

            var user = await hybridCache.GetOrCreateAsync<User>(cacheKey, async (entry) =>
            {
                logger.LogInformation("User with ID {UserId} not found in cache. Fetching from database.", userId);
                return await context.Users
                                 .Include(u => u.Roles)
                                 .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: entry) ?? null!;
            });

            return user!;
        }

        public async Task<List<User>> GetAllAsync(ISpecification<User>? specification = null!)
        {
            logger.LogInformation("Fetching all users with specification: {Specification}", specification);
            var query = context.Users.AsQueryable();

            if (specification == null) return await query.ToListAsync();
            query = query.Where(specification.Criteria);

            query = specification.Include(query);

            query = query.OrderBy(specification.OrderBy);

            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            logger.LogInformation("Adding new user: {User}", user);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            logger.LogInformation("Updating user: {User}", user);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            logger.LogInformation("Deleting user: {User}", user);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(Guid roleId, ISpecification<User>? specification = null!)
        {
            var cacheKey = $"usersRole_{roleId}";
            logger.LogInformation("Fetching users with role ID {RoleId} from cache or database.", roleId);

            var query = context.Users
                .Where(u => u.Roles!.Any(r => r.Id == roleId))
                .Include(u => u.Roles);

            if (specification == null) return await query.ToListAsync();
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Role>?>)
                query.Where(specification.Criteria);

            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Role>?>)query.OrderBy(specification.OrderBy);

            if (specification.Skip.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Role>?>)
                    query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Role>?>)
                    query.Take(specification.Take.Value);
            }

            var results = await hybridCache.GetOrCreateAsync<List<User>>(cacheKey, async (entry) =>
            {
                logger.LogInformation("Users with role ID {RoleId} not found in cache. Fetching from database.", roleId);
                return await query.ToListAsync(cancellationToken: entry);
            });

            return results!;
        }

        public async Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, ISpecification<User>? specification = null!)
        {
            var cacheKey = $"usersPerms_{permissionId}";
            logger.LogInformation("Fetching users with permission ID {PermissionId} from cache or database.", permissionId);

            var query = context.Users
                                .Where(u => u.Roles!
                                    .SelectMany(r => r.Permissions!)
                                    .Any(p => p.Id == permissionId))
                                .Include(u => u.Roles!)
                                    .ThenInclude(r => r.Permissions);

            if (specification == null) return await query.ToListAsync();
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Permission>?>)
                query.Where(specification.Criteria);

            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Permission>?>)
                query.OrderBy(specification.OrderBy);

            if (specification.Skip.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Permission>?>)
                    query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<User, ICollection<Permission>?>)
                    query.Take(specification.Take.Value);
            }

            var results = await hybridCache.GetOrCreateAsync<List<User>>(cacheKey, async (entry) =>
            {
                logger.LogInformation("Users with permission ID {PermissionId} not found in cache. Fetching from database.", permissionId);
                return await query.ToListAsync(cancellationToken: entry);
            });

            return results!;
        }
    }
}
