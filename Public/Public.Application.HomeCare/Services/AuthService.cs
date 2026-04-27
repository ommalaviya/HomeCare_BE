using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Public.Application.HomeCare.Interfaces;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Public.Application.HomeCare.Services
{
    public class AuthService(IConfiguration configuration) : IAuthService
    {
        public string GenerateToken(User user) => GenerateJwtToken(user);

        public string GenerateJwtToken(User user)
        {
            var jwtSection = configuration.GetSection("Jwt");

            var key = jwtSection["Key"]
                ?? throw new InvalidOperationException(
                    string.Format(Messages.NotConfigured, Messages.JwtKey));
            var issuer = jwtSection["Issuer"]
                ?? throw new InvalidOperationException(
                    string.Format(Messages.NotConfigured, Messages.JwtIssuer));
            var audience = jwtSection["Audience"]
                ?? throw new InvalidOperationException(
                    string.Format(Messages.NotConfigured, Messages.JwtAudience));

            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "1");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Name ?? string.Empty),
                new Claim("isEmailVerified", user.IsEmailVerified.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }
}
