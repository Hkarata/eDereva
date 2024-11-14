using eDereva.Core.Entities;

namespace eDereva.Core.Extensions
{
    /// <summary>
    /// Extension methods for working with permissions.
    /// </summary>
    public static class PermissionExtensions
    {
        /// <summary>
        /// Checks if the permissions include a specific permission flag.
        /// </summary>
        public static bool HasPermission(this Permission permission, PermissionFlag flag)
            => permission.Flags.HasFlag(flag);

        /// <summary>
        /// Creates a Permission object from permission flags.
        /// </summary>
        public static Permission CreateFromFlags(PermissionFlag flags, Guid roleId) => new()
        {
            RoleId = roleId,
            Flags = flags
        };
    }
}
