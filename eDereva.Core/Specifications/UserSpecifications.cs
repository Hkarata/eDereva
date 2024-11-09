using System.Linq.Expressions;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Core.Specifications
{
    public class UsersSpecification : ISpecification<User>
    {
        private readonly IRoleRepository _roleRepository;

        public Expression<Func<User, bool>>? Criteria { get; private set; }

        // Using expression-bodied members for cleaner syntax
        public Expression<Func<User, object>> OrderBy => user => user.FirstName;
        public Expression<Func<User, object>> OrderByDescending => null!;

        // Pagination properties
        public int? Take { get; private set; }
        public int? Skip { get; private set; }

        // Include related data
        public Func<IQueryable<User>, IQueryable<User>> Include => query =>
            query.Include(u => u.Roles)!
                 .ThenInclude(r => r.Permissions);

        public bool IgnoreSoftDeletes { get; }

        public UsersSpecification(
            IEnumerable<Guid> userRoleIds,
            bool includeDeleted,
            IRoleRepository roleRepository, bool ignoreSoftDeletes, int pageSize = 10,
            int pageNumber = 1)
        {
            _roleRepository = roleRepository;
            IgnoreSoftDeletes = ignoreSoftDeletes;

            // Initialize pagination
            Take = pageSize;
            Skip = (pageNumber - 1) * pageSize;

            // Optimize role permission check by loading all relevant roles at once
            InitializeCriteriaAsync(userRoleIds, includeDeleted).Wait();
        }

        private async Task InitializeCriteriaAsync(IEnumerable<Guid> userRoleIds, bool includeDeleted)
        {
            // Cache userRoleIds to avoid multiple enumerations
            var roleIds = userRoleIds.ToList();

            // Batch load all relevant roles with their permissions
            var rolesWithPermissions = await _roleRepository.GetRolesWithPermissionsAsync(roleIds);

            var canViewSoftDeleted = rolesWithPermissions.Any(role =>
                role.Permissions?.Any(p => p.CanViewSoftDeletedUsers) ?? false);

            // Set criteria based on permissions
            Criteria = BuildCriteria(canViewSoftDeleted || includeDeleted);
        }

        private static Expression<Func<User, bool>> BuildCriteria(bool includeDeleted)
        {
            return includeDeleted
                ? user => true
                : user => !user.IsDeleted;
        }
    }

}