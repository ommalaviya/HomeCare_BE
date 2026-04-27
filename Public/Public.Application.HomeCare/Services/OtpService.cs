using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.Users;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Constants;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Cryptography;
using System.Text;

namespace Public.Application.HomeCare.Services
{
    public class OtpService(
        IGenericRepository<Otp> genericRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IOtpRepository otpRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        IAuthService authService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
        : GenericService<Otp>(genericRepository, unitOfWork, mapper, principal), IOtpService
    {
        private HttpResponse Response => httpContextAccessor.HttpContext!.Response;
        private HttpRequest Request => httpContextAccessor.HttpContext!.Request;

        private string HashValue(string plainValue)
        {
            var secretKey = configuration["OtpSettings:SecretKey"]
                ?? throw new InvalidOperationException(string.Format(Messages.NotConfigured, Messages.OtpSecretKey));

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var valueBytes = Encoding.UTF8.GetBytes(plainValue);
            var hash = HMACSHA256.HashData(keyBytes, valueBytes);
            return Convert.ToHexString(hash);
        }

        private async Task UpsertOtpAsync(string email, string hashedCode)
        {
            var existingOtp = await otpRepository.GetByEmailAsync(email);

            if (existingOtp != null)
            {
                existingOtp.Code = hashedCode;
                existingOtp.ExpiryAt = DateTime.UtcNow.AddMinutes(10);
                existingOtp.IsUsed = false;
                existingOtp.CreatedAt = DateTime.UtcNow;
                await UpdateAsync(existingOtp);
            }
            else
            {
                await AddAsync(new Otp
                {
                    Email = email,
                    Code = hashedCode,
                    ExpiryAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private static void ThrowIfAccountInactive(User? user)
        {
            if (user == null) return;

            if (string.Equals(user.Status, "Block", StringComparison.OrdinalIgnoreCase))
                throw new AccountInactiveException(Messages.AccountBlocked);
        }

        public async Task SendOtpAsync(string email)
        {
            var activeUser = await userRepository.GetByEmailAsync(email);
            ThrowIfAccountInactive(activeUser);

            var plainCode = new Random().Next(1000, 9999).ToString();
            var hashedCode = HashValue(plainCode);

            await UpsertOtpAsync(email, hashedCode);

            await emailService.SendAsync(
                email,
                "Your OTP Code",
                $"Your OTP is {plainCode}. Valid for 10 minutes."
            );
        }

        public async Task<AuthResponseModel?> VerifyOtpAsync(string email, string code, string? name)
        {
            var hashedCode = HashValue(code);
            var otp = await otpRepository.GetValidOtpAsync(email, hashedCode);

            if (otp == null)
                return null;

            otp.IsUsed = true;
            await otpRepository.UpdateAsync(otp);

            var activeUser = await userRepository.GetByEmailAsync(email);
            ThrowIfAccountInactive(activeUser);

            User user;
            if (activeUser != null)
            {
                activeUser.IsEmailVerified = true;
                if (!string.IsNullOrWhiteSpace(name))
                    activeUser.Name = name;
                
                await userRepository.UpdateAsync(activeUser);

                user = activeUser;
            }
            else
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    CreatedAt = DateTime.UtcNow,
                    IsEmailVerified = true
                };
                await userRepository.AddAsync(user);
                await unitOfWork.SaveChangesAsync();
            }
            return await SaveAndReturnTokens(otp, user);
        }

        public async Task<AuthResponseModel?> RefreshTokenAsync()
        {
            var rawRefreshToken = Request.Cookies[AuthConstants.RefreshTokenCookie];

            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                return null;

            var hashedRefreshToken = HashValue(rawRefreshToken);
            var otp = await otpRepository.GetByRefreshTokenAsync(hashedRefreshToken);

            if (otp == null)
                return null;

            var user = await userRepository.GetByEmailAsync(otp.Email);
            if (user == null)
                return null;

            return await SaveAndReturnTokens(otp, user);
        }

        public void Logout()
        {
            DeleteAuthCookies();
        }

        private async Task<AuthResponseModel> SaveAndReturnTokens(Otp otp, User user)
        {
            var jwtExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60");
            var refreshExpiryMinutes = int.Parse(configuration["Jwt:RefreshExpiryMinutes"] ?? "1440");

            var refreshExpiresAt = DateTime.UtcNow.AddMinutes(refreshExpiryMinutes);

            var jwtToken = authService.GenerateJwtToken(user);
            var plainRefreshToken = authService.GenerateRefreshToken();

            otp.RefreshTokenHash = HashValue(plainRefreshToken);
            otp.RefreshTokenExpiryAt = refreshExpiresAt;
            await UpdateAsync(otp);

            SetAuthCookies(jwtToken, plainRefreshToken, jwtExpiryMinutes, refreshExpiryMinutes);

            return new AuthResponseModel
            {
                Token = jwtToken,
                User = Map<GetUserResponseModel, User>(user)
            };
        }

        private void SetAuthCookies(string accessToken, string refreshToken, int accessExpiry, int refreshExpiry)
        {
            Response.Cookies.Append(AuthConstants.AccessTokenCookie, accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpiry)
            });

            Response.Cookies.Append(AuthConstants.RefreshTokenCookie, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/api/otp",
                Expires = DateTimeOffset.UtcNow.AddMinutes(refreshExpiry)
            });
        }

        private void DeleteAuthCookies()
        {
            Response.Cookies.Delete(AuthConstants.AccessTokenCookie);
            Response.Cookies.Delete(AuthConstants.RefreshTokenCookie,
                new CookieOptions { Path = "/api/otp" });
        }
    }
}