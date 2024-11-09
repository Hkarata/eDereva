using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await context.Users
                                 .Include(u => u.Roles)
                                 .FirstOrDefaultAsync(u => u.Id == userId) ?? null!;
        }

        public async Task<List<User>> GetAllAsync(ISpecification<User>? specification = null!)
        {
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
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(Guid roleId, ISpecification<User>? specification = null!)
        {
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

            return await query.ToListAsync();
        }

        public async Task<List<User>> GetUsersWithPermissionsAsync(int permissionId, ISpecification<User>? specification = null!)
        {
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

            return await query.ToListAsync();
        }
    }
}
