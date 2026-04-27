using Admin.Application.HomeCare.Interfaces;
using AutoMapper;
using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Admin.Domain.HomeCare.DataModels.Response.Auth;
using Admin.Domain.HomeCare.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.HomeCare.Constants;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class AuthService(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IConfiguration configuration,
        IEmailService emailService,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env)
        : GenericService<AdminUser>(authRepository, unitOfWork, mapper, principal), IAuthService
    {
        private HttpResponse Response => httpContextAccessor.HttpContext!.Response;
        private HttpRequest Request => httpContextAccessor.HttpContext!.Request;

        private bool IsSecureCookie => !env.IsDevelopment();


        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel model)
        {
            var admin = await authRepository.GetByEmailAsync(model.Email);

            if (admin is null)
                throw new InvalidCredentialsException(Messages.InvalidCredentials);

            if (admin.IsDeleted)
                throw new AccountInactiveException(Messages.AccountInactive);

            if (!BCrypt.Net.BCrypt.Verify(model.Password, admin.PasswordHash))
                throw new InvalidCredentialsException(Messages.InvalidCredentials);

            var tokens = await tokenService.IssueTokensAsync(admin);

            SetAuthCookies(tokens.AccessToken, tokens.RefreshToken);

            var response = ToResponseModel<LoginResponseModel>(admin);
            response.Token = tokens.AccessToken;
            response.RefreshToken = tokens.RefreshToken;

            return response;
        }

        public async Task<LoginResponseModel> RefreshTokenAsync()
        {
            var rawAccessToken = Request.Cookies[AuthConstants.AccessTokenCookie];

            if (!string.IsNullOrWhiteSpace(rawAccessToken) && IsAccessTokenValid(rawAccessToken))
            {
                var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
                var jwt = handler.ReadJwtToken(rawAccessToken);
                var adminId = int.Parse(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

                var admin = await authRepository.GetByIdAsync(adminId)
                    ?? throw new TokenException(Messages.RefreshTokenInvalid);

                var freshAccessToken = tokenService.GenerateAccessToken(admin); // ← FIXED
                SetAccessTokenCookie(freshAccessToken);

                var response = ToResponseModel<LoginResponseModel>(admin);
                response.Token = freshAccessToken;
                return response;
            }

            var rawRefreshToken = Request.Cookies[AuthConstants.RefreshTokenCookie];

            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                throw new TokenException(Messages.RefreshTokenMissing);

            var result = await tokenService.RotateRefreshTokenAsync(rawRefreshToken);

            SetAuthCookies(result.AccessToken, result.RefreshToken);

            var rotatedAdmin = await authRepository.GetByIdAsync(result.AdminId)
                ?? throw new TokenException(Messages.RefreshTokenInvalid);

            var rotatedResponse = ToResponseModel<LoginResponseModel>(rotatedAdmin);
            rotatedResponse.Token = result.AccessToken;
            rotatedResponse.RefreshToken = result.RefreshToken;

            return rotatedResponse;
        }


        public async Task RevokeSessionAsync(int adminId)
        {
            await tokenService.RevokeAllTokensAsync(adminId);
            DeleteAuthCookies();
        }

        // Forgot Password

        public async Task<string> ForgotPasswordAsync(ForgotPasswordRequestModel model)
        {
            var admin = await authRepository.GetByEmailAsync(model.Email);

            if (admin is null || admin.IsDeleted)
                throw new KeyNotFoundException(Messages.EmailNotFound);

            await authRepository.InvalidateAllResetTokensAsync(admin.Id);

            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var expiryMinutes = int.Parse(configuration["ResetToken:ExpiryMinutes"] ?? "10");

            await authRepository.AddResetTokenAsync(new AdminPasswordResetToken
            {
                AdminId = admin.Id,
                Token = rawToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                IsUsed = false
            });

            await unitOfWork.SaveChangesAsync();

            var frontendUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
            var resetLink = $"{frontendUrl}/auth/reset-password?token={rawToken}&email={Uri.EscapeDataString(admin.Email)}";

            await emailService.SendResetPasswordEmailAsync(
                admin.Email, admin.Name, resetLink, expiryMinutes);

            return Messages.ForgotPasswordSent;
        }


        public async Task ValidateResetTokenAsync(string token)
        {
            var tokenRecord = await authRepository.GetValidResetTokenAsync(token);

            if (tokenRecord is null)
                throw new ResetTokenException(Messages.ResetLinkInvalid);

            if (tokenRecord.IsUsed)
                throw new ResetTokenException(Messages.ResetLinkUsed);

            if (tokenRecord.ExpiresAt < DateTime.UtcNow)
                throw new ResetTokenException(Messages.ResetLinkExpired);
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordRequestModel model)
        {
            var tokenRecord = await authRepository.GetValidResetTokenAsync(model.Token);

            if (tokenRecord is null)
                throw new ResetTokenException(Messages.ResetLinkInvalid);

            if (tokenRecord.IsUsed)
                throw new ResetTokenException(Messages.ResetLinkUsed);

            if (tokenRecord.ExpiresAt < DateTime.UtcNow)
                throw new ResetTokenException(Messages.ResetLinkExpired);

            var admin = tokenRecord.Admin;

            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, admin.PasswordHash))
                throw new ResetTokenException(Messages.NewPasswordSameAsOld);

            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            tokenRecord.IsUsed = true;

            await authRepository.UpdateAsync(admin);
            await tokenService.RevokeAllTokensAsync(admin.Id);
            return Messages.PasswordResetSuccess;
        }

        // Cookie helpers

        private void SetAuthCookies(string accessToken, string refreshToken)
        {
            SetAccessTokenCookie(accessToken);

            var refreshExpiry = int.Parse(configuration["JwtSettings:RefreshTokenExpiryMinutes"] ?? "1440");
            Response.Cookies.Append(AuthConstants.RefreshTokenCookie, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = IsSecureCookie,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth",
                Expires = DateTimeOffset.UtcNow.AddMinutes(refreshExpiry)
            });
        }

        // Separate method so silent refresh can set only the access cookie
        private void SetAccessTokenCookie(string accessToken)
        {
            var accessExpiry = int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60");
            Response.Cookies.Append(AuthConstants.AccessTokenCookie, accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = IsSecureCookie,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpiry)
            });
        }

        private void DeleteAuthCookies()
        {
            Response.Cookies.Delete(AuthConstants.AccessTokenCookie);
            Response.Cookies.Delete(AuthConstants.RefreshTokenCookie,
                new CookieOptions { Path = "/api/auth" });
        }

        // JWT validation helper

        private bool IsAccessTokenValid(string token)
        {
            try
            {
                var jwt = configuration.GetSection("JwtSettings");
                var key = jwt["SecretKey"] ?? throw new InvalidOperationException();
                var handler = new JwtSecurityTokenHandler();

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                                              Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}