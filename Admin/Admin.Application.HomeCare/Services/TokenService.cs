using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Response.Auth;
using Admin.Domain.HomeCare.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Services
{
    public class TokenService(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration) : ITokenService
    {
        public async Task<IssueTokensResultModel> IssueTokensAsync(AdminUser admin)
        {
            var accessToken = GenerateAccessToken(admin);
            var (rawRefreshToken, hash) = CreateRefreshToken();

            var now = DateTime.UtcNow;
            await authRepository.AddRefreshTokenAsync(new AdminRefreshToken
            {
                AdminId = admin.Id,
                TokenHash = hash,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(GetRefreshExpiryMinutes())
            });

            await unitOfWork.SaveChangesAsync();

            return new IssueTokensResultModel
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken
            };
        }

        public async Task<RotateTokensResultModel> RotateRefreshTokenAsync(string rawRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                throw new TokenException(Messages.RefreshTokenMissing);

            var tokenHash = HashToken(rawRefreshToken);
            var stored = await authRepository.GetActiveRefreshTokenAsync(tokenHash);

            if (stored is null)
            {
                var recentlyRotated = await authRepository.GetRefreshTokenByHashAsync(tokenHash);

                if (recentlyRotated is { IsRevoked: true, ReplacedByTokenHash: not null })
                {
                    var replacement = await authRepository.GetActiveRefreshTokenAsync(recentlyRotated.ReplacedByTokenHash);

                    if (replacement is not null && !replacement.Admin.IsDeleted)
                    {
                        return new RotateTokensResultModel
                        {
                            AccessToken = GenerateAccessToken(replacement.Admin),
                            RefreshToken = rawRefreshToken,
                            AdminId = replacement.Admin.Id
                        };
                    }
                }

                throw new TokenException(Messages.RefreshTokenInvalid);
            }

            var admin = stored.Admin;

            if (admin.IsDeleted)
                throw new TokenException(Messages.RefreshTokenInvalid);

            stored.IsRevoked = true;
            var (newRawToken, newHash) = CreateRefreshToken();
            stored.ReplacedByTokenHash = newHash;
            await authRepository.UpdateRefreshTokenAsync(stored);

            var rotatedAt = DateTime.UtcNow;
            await authRepository.AddRefreshTokenAsync(new AdminRefreshToken
            {
                AdminId = admin.Id,
                TokenHash = newHash,
                CreatedAt = rotatedAt,
                ExpiresAt = rotatedAt.AddMinutes(GetRefreshExpiryMinutes())
            });

            await unitOfWork.SaveChangesAsync();

            return new RotateTokensResultModel
            {
                AccessToken = GenerateAccessToken(admin),
                RefreshToken = newRawToken,
                AdminId = admin.Id
            };
        }

        public async Task RevokeAllTokensAsync(int adminId)
        {
            await authRepository.RevokeAllRefreshTokensAsync(adminId);
            await unitOfWork.SaveChangesAsync();
        }

        public string GenerateAccessToken(AdminUser admin)
        {
            var jwt = configuration.GetSection("JwtSettings");
            var key = jwt["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing");
            var expiry = int.Parse(jwt["ExpiryMinutes"] ?? "60");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(ClaimNames.Name,         admin.Name),
                new Claim(ClaimNames.IsSuperAdmin, admin.IsSuperAdmin.ToString().ToLower()),
                new Claim(ClaimNames.IsDeleted,    admin.IsDeleted.ToString().ToLower()),
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetRefreshExpiryMinutes()
            => int.Parse(configuration["JwtSettings:RefreshTokenExpiryMinutes"] ?? "1440");

        private static (string raw, string hash) CreateRefreshToken()
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return (raw, HashToken(raw));
        }

        private static string HashToken(string raw)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}