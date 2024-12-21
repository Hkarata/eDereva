using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eDereva.Core.Enums;
using eDereva.Core.Services;

namespace eDereva.Api.Extensions;

public class JwtRefreshMiddleware(RequestDelegate next, ITokenService tokenService)
{
    private const int MaxTokenRefreshAllowed = 5;

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization;
        var token = string.Empty;

        if (authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = authorizationHeader.ToString()["Bearer ".Length..];

        if (string.IsNullOrEmpty(token))
        {
            await next(context);
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jsonWebToken;

        // Try parsing the token
        try
        {
            jsonWebToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonWebToken == null || !ValidateToken(jsonWebToken, context))
                return;
        }
        catch (Exception)
        {
            context.Response.StatusCode = 400; // Bad request: Invalid token format
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Token is malformed.\"}");
            return;
        }

        // If token is close to expiring, refresh it
        if (tokenService.IsTokenCloseToExpiring(jsonWebToken))
            try
            {
                var newToken = RefreshToken(jsonWebToken);
                context.Response.Headers.Append("X-New-Token", newToken);
            }
            catch
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Max token refresh limit exceeded.\"}");
                return;
            }

        // Proceed to the next middleware in the pipeline
        await next(context);
    }

    // Validates the token and returns a boolean indicating whether it's valid
    private static bool ValidateToken(JwtSecurityToken? token, HttpContext context)
    {
        // Check if the token is expired
        if (token != null && token.ValidTo >= DateTime.UtcNow) return true;
        context.Response.StatusCode = 401; // Unauthorized
        context.Response.ContentType = "application/json";
        context.Response.WriteAsync("{\"error\": \"Token is expired.\"}");
        return false;
    }

    // Refreshes the token and returns the new token as a string
    private string RefreshToken(JwtSecurityToken? jsonWebToken)
    {
        var nin = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
        if (string.IsNullOrEmpty(nin)) throw new InvalidOperationException("Missing NIN claim.");

        var givenName = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var surname = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        var phoneNumber = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
        var email = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var refreshTimes = jsonWebToken?.Claims.FirstOrDefault(c => c.Type == "RefreshTimes")?.Value;
        var refreshTimesInt = int.TryParse(refreshTimes, out var times) ? times : 0;

        if (refreshTimesInt > MaxTokenRefreshAllowed)
            throw new InvalidOperationException("Max token refresh limit exceeded.");

        var permissionsClaims = jsonWebToken?.Claims
            .Where(c => c.Type == "Permission")
            .Select(c => Enum.TryParse<PermissionFlag>(c.Value, out var flag) ? flag : PermissionFlag.None)
            .ToArray();

        var combinedPermissions =
            permissionsClaims?.Aggregate(PermissionFlag.None, (current, flag) => current | flag) ?? PermissionFlag.None;

        // Generate new token
        return tokenService.RefreshToken(nin, givenName!, surname!, phoneNumber!, email!, combinedPermissions,
            refreshTimesInt + 1);
    }
}