using System.IdentityModel.Tokens.Jwt;

namespace eDereva.Core.Services
{
    public interface ITokenService
    {
        string GenerateToken(string username, List<string> roles);
        bool IsTokenCloseToExpiring(JwtSecurityToken token);
    }
}
