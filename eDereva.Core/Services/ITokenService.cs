using System.IdentityModel.Tokens.Jwt;
using eDereva.Core.Enums;

namespace eDereva.Core.Services;

/// <summary>
///     Defines methods for generating and validating JWT tokens with permission-based claims.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Generates a JWT token for a user with the specified permissions.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="permissions">The permission flags assigned to the user.</param>
    /// <returns>A JWT token as a string.</returns>
    string GenerateToken(string username, PermissionFlag permissions);

    /// <summary>
    ///     Checks if the given token is close to expiring.
    /// </summary>
    /// <param name="token">The JWT token to check.</param>
    /// <returns>True if the token is close to expiring; otherwise, false.</returns>
    bool IsTokenCloseToExpiring(JwtSecurityToken token);
}