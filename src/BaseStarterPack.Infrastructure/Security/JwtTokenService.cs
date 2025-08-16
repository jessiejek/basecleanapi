using BaseStarterPack.Application.Interfaces.Services;
using BaseStarterPack.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaseStarterPack.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config) => _config = config;

        public (string token, DateTime expiresAtUtc) CreateAccessToken(User user)
        {
            var secret = _config["Jwt:Secret"] ?? "dev-insecure-key-change-me";
            var issuer = _config["Jwt:Issuer"] ?? "BaseStarterPack";
            var audience = _config["Jwt:Audience"] ?? "BaseStarterPack";
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 15;

            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new("name", $"{user.FirstName} {user.LastName}".Trim())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (token, expires);
        }

        public RefreshToken CreateRefreshToken(User user)
        {
            var days = int.TryParse(_config["Jwt:RefreshTokenDays"], out var d) ? d : 7;

            // EXPLICITLY set NOT NULL columns so insert never sends NULLs
            return new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateBase64UrlToken(32),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(days),
                CreatedAtUtc = DateTime.UtcNow, // your DB requires NOT NULL
                Revoked = false            // your DB requires NOT NULL
            };
        }

        private static string GenerateBase64UrlToken(int bytesLength)
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(bytesLength);
            return Convert.ToBase64String(bytes)
                          .TrimEnd('=')
                          .Replace('+', '-')
                          .Replace('/', '_');
        }
    }
}
