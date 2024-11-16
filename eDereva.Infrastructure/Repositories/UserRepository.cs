using eDereva.Core.Contracts.Responses;
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

        public async Task<User> GetByIdAsync(string nin, CancellationToken cancellationToken)
        {
            var cacheKey = $"User_{nin}";
            logger.LogInformation("Fetching user with ID {UserId} from cache or database.", nin);

            var user = await hybridCache.GetOrCreateAsync<User>(cacheKey, async (entry) =>
            {
                logger.LogInformation("User with ID {UserId} not found in cache. Fetching from database.", nin);
                return await context.Users
                                 .Include(u => u.Roles)
                                 .FirstOrDefaultAsync(u => u.Nin == nin, cancellationToken: entry) ?? null!;
            }, cancellationToken: cancellationToken);

            return user;
        }

        public async Task<PaginatedResult<UserDto>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            var query = context.Users
                .AsNoTracking()
                .AsSplitQuery();

            // Get the total count of users
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and retrieve the users
            var users = await query
                .OrderBy(u => u.LastName) // Adjust ordering based on requirements
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(u => new UserDto
                {
                    Nin = u.Nin,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName!,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,
                    Sex = u.Sex.ToString()
                })
                .ToListAsync(cancellationToken);

            // Return paginated result
            return new PaginatedResult<UserDto>(users, totalCount, pagination);
        }


        public async Task<bool> AddAsync(User user, CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding new user: {User}", user);
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            logger.LogInformation("Updating user: {User}", user);
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task DeleteAsync(string nin, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(nin, cancellationToken);
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        public Task<List<User>> GetUsersByRoleAsync(Guid roleId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
