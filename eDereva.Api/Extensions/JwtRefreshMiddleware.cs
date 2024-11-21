using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eDereva.Core.Enums;
using eDereva.Core.Services;

namespace eDereva.Api.Extensions;

public class JwtRefreshMiddleware(RequestDelegate next, ITokenService tokenService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (authorizationHeader.StartsWith("Bearer "))
            {
                var tokenString = authorizationHeader.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();

                if (handler.ReadToken(tokenString) is JwtSecurityToken token &&
                    tokenService.IsTokenCloseToExpiring(token))
                {
                    var username = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var permissionsClaim = token.Claims.FirstOrDefault(c => c.Type == "Permissions")?.Value;

                    if (username != null && permissionsClaim != null &&
                        Enum.TryParse(permissionsClaim, out PermissionFlag permissions))
                    {
                        var newToken = tokenService.GenerateToken(username, permissions);
                        context.Response.Headers.Append("X-New-Token",
                            newToken); // Include new token in response header
                    }
                }
            }
        }

        await next(context);
    }
}