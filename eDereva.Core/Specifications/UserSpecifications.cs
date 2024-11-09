using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class UsersSpecification : ISpecification<User>
{
    // Filtering criteria: check if the user is deleted or not
    public Expression<Func<User, bool>> Criteria { get; }

    // Ordering criteria (optional)
    public Expression<Func<User, object>> OrderBy => user => user.FirstName;
    public Expression<Func<User, object>> OrderByDescending => null!;

    // Pagination (optional)
    public int? Take { get; }
    public int? Skip { get; }

    // Include related data (optional)
    public Func<IQueryable<User>, IQueryable<User>> Include => query => query.Include(u => u.Roles);

    // Flag to ignore soft deletes (optional)
    public bool IgnoreSoftDeletes { get; }

    public UsersSpecification(IEnumerable<int> userRoleIds, bool includeDeleted)
    {
        // If user has permission or includeDeleted flag is set to true, include soft-deleted users
        bool canViewSoftDeleted = userRoleIds.Any(roleId =>
            // Assuming you have a method to check permissions for the given role
            HasPermissionForRole(roleId, "CanViewSoftDeletedUsers"));

        // Set criteria based on whether they can view soft-deleted users or not
        if (canViewSoftDeleted || includeDeleted)
        {
            Criteria = user => true; // No filter, allow all users
        }
        else
        {
            Criteria = user => user.IsDeleted == false; // Filter out deleted users
        }

        // Set pagination values if needed
        Take = 10;  // Example: limit the result set to 10 users per page
        Skip = 0;   // Example: no skipping, starting from the first user
    }

    private bool HasPermissionForRole(int roleId, string permissionName)
    {
        // Placeholder logic to check permissions for a role
        // In real-world, you'd query the permissions associated with the role from the DB
        return permissionName == "CanViewSoftDeletedUsers";  // Simplified check
    }
}