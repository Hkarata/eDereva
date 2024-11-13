using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eDereva.Core.Services;

namespace eDereva.Api.Extensions
{
    public class JwtRefreshMiddleware(RequestDelegate next, ITokenService tokenService)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tokenString = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();

                if (handler.ReadToken(tokenString) is JwtSecurityToken token && tokenService.IsTokenCloseToExpiring(token))
                {
                    var username = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                    if (username != null && roles.Count != 0)
                    {
                        var newToken = tokenService.GenerateToken(username, roles); // Generate token with multiple roles
                        context.Response.Headers.Append("X-New-Token", newToken); // Include new token in response header
                    }
                }
            }

            await next(context);
        }
    }
}