using System.IdentityModel.Tokens.Jwt;
using eDereva.Core.Enums;

namespace eDereva.Core.Services;

/// <summary>
///     Defines methods for generating and validating JWT tokens with permission-based claims.
/// </summary>
public interface ITokenService
{
    string GenerateToken(string nin, string givenName, string surname, string phoneNumber, string email,
        PermissionFlag permissions);

    /// <summary>
    ///     Checks if the given token is close to expiring.
    /// </summary>
    /// <param name="token">The JWT token to check.</param>
    /// <returns>True if the token is close to expiring; otherwise, false.</returns>
    bool IsTokenCloseToExpiring(JwtSecurityToken? token);

    string RefreshToken(string nin, string givenName, string surname, string phoneNumber, string email,
        PermissionFlag permissions, int refreshTimes);
}