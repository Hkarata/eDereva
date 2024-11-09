using System.Linq.Expressions;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Core.Specifications
{
    public class UsersSpecification(IRoleRepository roleRepository) : ISpecification<User>
    {
        public Expression<Func<User, bool>> Criteria { get; private set; } = default!;
        public Expression<Func<User, object>> OrderBy => user => user.FirstName;
        public Expression<Func<User, object>> OrderByDescending => null!;

        public int? Take { get; private set; }
        public int? Skip { get; private set; }

        public Func<IQueryable<User>, IQueryable<User>> Include => query =>
            query.Include(u => u.Roles)!
                 .ThenInclude(r => r.Permissions);

        public bool IgnoreSoftDeletes { get; private set; }

        public async Task InitializeAsync(bool includeDeleted, int pageSize = 10, int pageNumber = 1)
        {
            // Set pagination values
            Take = pageSize;
            Skip = (pageNumber - 1) * pageSize;

            // Get role IDs for permissions check
            var roleIds = await roleRepository.GetRoleIdsAsync();

            // Load roles and permissions in a batch to avoid multiple calls
            var rolesWithPermissions = await roleRepository.GetRolesWithPermissionsAsync(roleIds);

            // Check if any role has permission for viewing soft-deleted users
            var canViewSoftDeleted = rolesWithPermissions.Any(role =>
                role.Permissions?.Any(p => p.CanViewSoftDeletedUsers) ?? false);

            // Set the criteria for filtering users
            Criteria = BuildCriteria(canViewSoftDeleted || includeDeleted);
        }

        private static Expression<Func<User, bool>> BuildCriteria(bool includeDeleted)
        {
            // If soft deletes should be included, allow all; otherwise, exclude soft-deleted users
            return includeDeleted
                ? user => true
                : user => !user.IsDeleted;
        }
    }
}
