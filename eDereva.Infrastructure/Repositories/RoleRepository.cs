using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Infrastructure.Repositories
{
    public class RoleRepository(ApplicationDbContext context, HybridCache cache) : IRoleRepository
    {
        public async Task<Role> GetByIdAsync(Guid roleId)
        {
            return await context.Roles
                .Where(r => r.Id == roleId)
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync() ?? null!;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task<Role> AddAsync(Role role)
        {
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            context.Roles.Update(role);
            await context.SaveChangesAsync();
            return role;
        }

        public async Task DeleteAsync(Role role)
        {
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
        }

        public Task<List<Role>> GetRolesWithPermissionsAsync(IEnumerable<Guid> roleIds)
        {
            throw new NotImplementedException();
        }
    }
}
