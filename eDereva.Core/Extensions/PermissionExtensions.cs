using eDereva.Core.Entities;
using eDereva.Core.Enums;

namespace eDereva.Core.Extensions;

/// <summary>
///     Extension methods for working with permissions.
/// </summary>
public static class PermissionExtensions
{
    /// <summary>
    ///     Checks if the permissions include a specific permission flag.
    /// </summary>
    public static bool HasPermission(this Permission permission, PermissionFlag flag)
    {
        return permission.Flags.HasFlag(flag);
    }

    /// <summary>
    ///     Creates a Permission object from permission flags.
    /// </summary>
    public static Permission CreateFromFlags(PermissionFlag flags, Guid roleId)
    {
        return new Permission
        {
            RoleId = roleId,
            Flags = flags
        };
    }
}