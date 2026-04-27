using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.AdminUser;
using Admin.Domain.HomeCare.DataModels.Response.AdminUser;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class AdminUserService(
        IAdminUserRepository repository,
        IEmailService emailService,
        ClaimsPrincipal principal,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : GenericService<AdminUser>(repository, unitOfWork, mapper, principal), IAdminUserService
    {
        private bool CallerIsSuperAdmin =>
            bool.TryParse(principal.FindFirstValue(ClaimNames.IsSuperAdmin), out var v) && v;

        public async Task<DataQueryResponseModel<GetAdminUserResponseModel>> GetAdminUsersAsync(
            FilterAdminUserRequestModel filter)
        {
            var predicate = BuildPredicate(filter);

            bool sortByName = string.Equals(filter.SortField, "Name", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(filter.SortField, "Username", StringComparison.OrdinalIgnoreCase);

            if (sortByName)
            {
                var fetchFilter = new FilterAdminUserRequestModel
                {
                    IsSuperAdmin = filter.IsSuperAdmin,
                    IsActive = filter.IsActive,
                    SortField = null,
                    SortDirection = null,
                    PageNumber = 0,
                    PageSize = 0
                };

                var all = await GetAllAsync(predicate: predicate, model: fetchFilter);
                var mapped = ToResponseModel<IEnumerable<GetAdminUserResponseModel>>(all.Records).ToList();

                var sorted = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                    ? mapped.OrderByDescending(x => x.Name).ToList()
                    : mapped.OrderBy(x => x.Name).ToList();

                var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
                var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

                return new DataQueryResponseModel<GetAdminUserResponseModel>
                {
                    TotalRecords = all.TotalRecords,
                    Records = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                };
            }

            var response = await GetAllAsync(predicate: predicate, model: filter);

            return new DataQueryResponseModel<GetAdminUserResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetAdminUserResponseModel>>(response.Records)
            };
        }

        private static System.Linq.Expressions.Expression<Func<AdminUser, bool>> BuildPredicate(
            FilterAdminUserRequestModel filter)
        {
            return x =>
                (filter.IsSuperAdmin == null || x.IsSuperAdmin == filter.IsSuperAdmin) &&
                (filter.IsActive == null || x.IsDeleted == !filter.IsActive);
        }

        public async Task<GetAdminUserResponseModel> GetAdminUserByIdAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id,
                string.Format(Messages.NotFound, Messages.Admin));

            return ToResponseModel<GetAdminUserResponseModel>(entity);
        }

        public async Task<GetAdminUserResponseModel> CreateAdminUserAsync(
            CreateAdminUserRequestModel request)
        {
            var deleted = await repository.GetDeletedByEmailAsync(request.Email!);
            if (deleted != null)
            {
                deleted.Name = request.Name!;
                deleted.MobileNumber = request.MobileNumber!;
                deleted.IsSuperAdmin = request.IsSuperAdmin;
                deleted.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                deleted.IsDeleted = false;

                await UpdateAsync(deleted);
                await emailService.SendAccountReactivatedEmailAsync(deleted.Email, deleted.Name, request.Password!);

                return ToResponseModel<GetAdminUserResponseModel>(deleted);
            }

            if (await repository.EmailExistsAsync(request.Email!))
                throw new DuplicateRecordException(Messages.EmailAlreadyExists);

            if (await repository.MobileExistsAsync(request.MobileNumber!))
                throw new DuplicateRecordException(Messages.MobileAlreadyExists);

            var entity = new AdminUser
            {
                Name = request.Name!,
                Email = request.Email!,
                MobileNumber = request.MobileNumber!,
                IsSuperAdmin = request.IsSuperAdmin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await AddAsync(entity);
            await emailService.SendWelcomeEmailAsync(entity.Email, entity.Name, request.Password!);

            return ToResponseModel<GetAdminUserResponseModel>(entity);
        }

        public async Task<GetAdminUserResponseModel> UpdateAdminUserAsync(
            UpdateAdminUserRequestModel request)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == request.Id,
                string.Format(Messages.NotFound, Messages.Admin));

            if (await repository.EmailExistsAsync(request.Email!, request.Id))
                throw new DuplicateRecordException(Messages.EmailAlreadyExists);

            if (await repository.MobileExistsAsync(request.MobileNumber!, request.Id))
                throw new DuplicateRecordException(Messages.MobileAlreadyExists);

            var changedFields = new Dictionary<string, string>();
            if (entity.Name != request.Name) changedFields["Name"] = request.Name!;
            if (entity.Email != request.Email) changedFields["Email"] = request.Email!;
            if (entity.MobileNumber != request.MobileNumber) changedFields["Mobile Number"] = request.MobileNumber!;

            entity.Name = request.Name!;
            entity.Email = request.Email!;
            entity.MobileNumber = request.MobileNumber!;
            entity.IsSuperAdmin = request.IsSuperAdmin;

            bool passwordChanged = false;
            string? newPlainPassword = null;

            if (CallerIsSuperAdmin && !string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password != request.ConfirmPassword)
                    throw new InvalidOperationException(Messages.PasswordsMustMatch);

                if (BCrypt.Net.BCrypt.Verify(request.Password, entity.PasswordHash))
                    throw new InvalidCredentialsException(Messages.NewPasswordSameAsOld);

                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                newPlainPassword = request.Password;
                passwordChanged = true;
            }

            await UpdateAsync(entity);

            if (changedFields.Count > 0)
                await emailService.SendAccountUpdatedEmailAsync(entity.Email, entity.Name, changedFields);

            if (passwordChanged)
                await emailService.SendPasswordChangedEmailAsync(entity.Email, entity.Name, newPlainPassword);

            return ToResponseModel<GetAdminUserResponseModel>(entity);
        }

        public async Task<bool> DeleteAdminUserAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Admin));

            var email = entity.Email;
            var name = entity.Name;

            await SoftDeleteAsync(entity);
            await emailService.SendAccountDeletedEmailAsync(email, name);

            return true;
        }

        public async Task ChangeAdminUserPasswordAsync(ChangeAdminUserPasswordRequestModel request)
        {
            if (!CallerIsSuperAdmin && request.TargetAdminId != CurrentUserId)
                throw new UnauthorizedAccessException("You can only change your own password.");

            var entity = await FindOrThrowAsync(
                x => x.Id == request.TargetAdminId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Admin));

            if (BCrypt.Net.BCrypt.Verify(request.Password, entity.PasswordHash))
                throw new InvalidCredentialsException(Messages.NewPasswordSameAsOld);

            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await UpdateAsync(entity);

            var plainPw = CallerIsSuperAdmin ? request.Password : null;
            await emailService.SendPasswordChangedEmailAsync(entity.Email, entity.Name, plainPw);
        }
    }
}