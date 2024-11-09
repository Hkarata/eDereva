using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eDereva.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace eDereva.Infrastructure.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        private readonly string _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException(nameof(configuration), "SecretKey cannot be null");
        private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "Issuer cannot be null");
        private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "Audience cannot be null");


        public string GenerateToken(string username, List<string> roles)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            if (roles == null || !roles.Any())
                throw new ArgumentException("Roles cannot be null or empty", nameof(roles));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username)
            };

            // Add each role as a separate claim
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Using UTC time
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool IsTokenCloseToExpiring(JwtSecurityToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            var timeToExpire = token.ValidTo - DateTime.UtcNow;
            return timeToExpire < TimeSpan.FromMinutes(5);
        }
    }

}