using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context, HybridCache hybridCache, ILogger<UserRepository> logger) : IUserRepository
    {

        public async Task<User> GetByIdAsync(string Nin)
        {
            var cacheKey = $"User_{Nin}";
            logger.LogInformation("Fetching user with ID {UserId} from cache or database.", Nin);

            var user = await hybridCache.GetOrCreateAsync<User>(cacheKey, async (entry) =>
            {
                logger.LogInformation("User with ID {UserId} not found in cache. Fetching from database.", Nin);
                return await context.Users
                                 .Include(u => u.Roles)
                                 .FirstOrDefaultAsync(u => u.NIN == Nin, cancellationToken: entry) ?? null!;
            });

            return user!;
        }

        public async Task<bool> AddAsync(User user)
        {
            logger.LogInformation("Adding new user: {User}", user);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<User> UpdateAsync(User user)
        {
            logger.LogInformation("Updating user: {User}", user);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(string Nin)
        {
            var user = await GetByIdAsync(Nin);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public Task<List<User>> GetAllAsync(PaginationParams pagination)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByRoleAsync(Guid roleId, PaginationParams pagination)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, PaginationParams pagination)
        {
            throw new NotImplementedException();
        }
    }
}
