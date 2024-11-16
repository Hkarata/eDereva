using eDereva.Core.Enums;

namespace eDereva.Core.Contracts.Requests
{
    public record RoleDto
        (
            string Name,
            string Description,
            PermissionFlag Permissions
        );
}
