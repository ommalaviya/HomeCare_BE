using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Public.Domain.HomeCare.DataModels.Response.Users;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services
{
    public class UserService(
        IGenericRepository<User> genericRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        IEmailService emailService,
        IConfiguration configuration)
        : GenericService<User>(genericRepository, unitOfWork, mapper, principal), IUserService
    {

        private async Task<User> GetUserOrThrowAsync(int userId)
            => await GetOrThrowAsync(userId, string.Format(Messages.NotFound, Messages.User));

        private async Task EnsureEmailNotTakenAsync(string email, int excludeUserId)
        {
            var existing = await userRepository.GetByEmailAsync(email);
            if (existing != null && existing.Id != excludeUserId)
                throw new InvalidOperationException(
                    string.Format(Messages.DuplicateRecord, Messages.Email));
        }

        private string HashOtpValue(string plainValue)
        {
            var secretKey = configuration["OtpSettings:SecretKey"]
                ?? throw new InvalidOperationException(Messages.OtpSecretKeyNotConfigured);

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
                await otpRepository.UpdateAsync(existingOtp);
                await UnitOfWork.SaveChangesAsync();
            }
            else
            {
                await otpRepository.AddAsync(new Otp
                {
                    Email = email,
                    Code = hashedCode,
                    ExpiryAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                });
                await UnitOfWork.SaveChangesAsync();
            }
        }

        public async Task<DataQueryResponseModel<GetUserResponseModel>> GetUsersAsync()
        {
            var response = await GetAllAsync(null);

            return new DataQueryResponseModel<GetUserResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetUserResponseModel>>(response.Records),
            };
        }

        public async Task<GetUserResponseModel?> GetUserByEmailAsync(string email)
        {
            var user = await userRepository.GetByEmailAsync(email);
            return user == null ? null : Map<GetUserResponseModel, User>(user);
        }

        public async Task<GetUserResponseModel> CreateOrUpdateUserAsync(
            CreateOrUpdateUserRequestModel request)
        {
            request.Email = request.Email!.ToLower();
            var existingUser = await userRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                existingUser.Name = request.Name;
                await UpdateAsync(existingUser);
                return Map<GetUserResponseModel, User>(existingUser);
            }

            var newUser = ToEntity(request);
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.IsEmailVerified = false;

            await AddAsync(newUser);
            return Map<GetUserResponseModel, User>(newUser);
        }

        public async Task<GetUserResponseModel> GetProfileAsync()
        {
            var user = await GetUserOrThrowAsync(CurrentUserId);
            return Map<GetUserResponseModel, User>(user);
        }

        public async Task SendEmailUpdateOtpAsync(string newEmail)
        {
            await GetUserOrThrowAsync(CurrentUserId);
            await EnsureEmailNotTakenAsync(newEmail, CurrentUserId);

            var plainCode = new Random().Next(1000, 9999).ToString();
            var hashedCode = HashOtpValue(plainCode);

            await UpsertOtpAsync(newEmail, hashedCode);

            await emailService.SendAsync(
                newEmail,
                "Verify your new email",
                $"Your OTP to update email is {plainCode}. Valid for 10 minutes.");
        }

        public async Task<GetUserResponseModel> UpdateEmailAsync(UpdateEmailRequestModel request)
        {
            var user = await GetUserOrThrowAsync(CurrentUserId);
            var hashedCode = HashOtpValue(request.Otp);

            var otp = await otpRepository.GetValidOtpAsync(request.NewEmail, hashedCode);
            if (otp == null)
                throw new UnauthorizedAccessException(Messages.InvalidOrExpiredOtp);

            otp.IsUsed = true;
            await otpRepository.UpdateAsync(otp);
            await UnitOfWork.SaveChangesAsync();

            var oldEmail = user.Email;

            user.Email = request.NewEmail;
            user.IsEmailVerified = true;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = CurrentUserId;
            await UpdateAsync(user);

            var activeRefreshRow = await otpRepository.GetActiveRefreshTokenRowByEmailAsync(oldEmail);
            if (activeRefreshRow != null)
            {
                activeRefreshRow.Email = request.NewEmail;
                await otpRepository.UpdateAsync(activeRefreshRow);
                await UnitOfWork.SaveChangesAsync();
            }

            return Map<GetUserResponseModel, User>(user);
        }

        public async Task<GetUserResponseModel> UpdatePhoneAsync(UpdatePhoneRequestModel request)
        {
            var user = await GetUserOrThrowAsync(CurrentUserId);

            user.MobileNumber = request.MobileNumber;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = CurrentUserId;
            await UpdateAsync(user);

            return Map<GetUserResponseModel, User>(user);
        }

    }
}