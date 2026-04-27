using Admin.Domain.HomeCare.DataModels.Request.Admin;
using Admin.Domain.HomeCare.DataModels.Response.Admin;
using Application.HomeCare.Interfaces;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Services;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper;
using System.Security.Claims;

public class AdminProfileService : GenericService<AdminUser>, IAdminProfileService
{
    private const string IMAGE_FOLDER = "admins";
    private readonly IFileService _fileService;

    public AdminProfileService(
        IGenericRepository<AdminUser> adminRepository,
        IFileService fileService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal
        )
        : base(adminRepository, unitOfWork, mapper, principal)
    {
        _fileService = fileService;
    }

    public async Task<AdminProfileResponse> GetProfileAsync()
    {
        var admin = await GetOrThrowAsync(CurrentUserId,
            string.Format(Messages.NotFound, Messages.Admin));

        return ToResponseModel<AdminProfileResponse>(admin);
    }

    public async Task UpdateContactAsync(UpdateAdminContactRequest model)
    {
        var admin = await GetOrThrowAsync(CurrentUserId,
            string.Format(Messages.NotFound, Messages.Admin));

        Map<AdminUser, UpdateAdminContactRequest>(model, admin);
        await UpdateAsync(admin);
    }

    public async Task UpdatePasswordAsync(UpdateAdminPasswordRequest model)
    {
        var admin = await GetOrThrowAsync(CurrentUserId,
            string.Format(Messages.NotFound, Messages.Admin));

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, admin.PasswordHash))
            throw new InvalidCredentialsException(Messages.CurrentPasswordIncorrect);

        if (BCrypt.Net.BCrypt.Verify(model.NewPassword, admin.PasswordHash))
            throw new InvalidCredentialsException(Messages.NewPasswordSameAsOld);

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        await UpdateAsync(admin);
    }

    public async Task<FileContentHttpResult> UpdateProfileImageAsync(IFormFile file)
    {
        var admin = await GetOrThrowAsync(CurrentUserId,
            string.Format(Messages.NotFound, Messages.Admin));

        string imageName = string.IsNullOrEmpty(admin.ProfileImageName)
            ? await _fileService.SaveImageAsync(file, IMAGE_FOLDER)
            : await _fileService.UpdateImageAsync(file, admin.ProfileImageName, IMAGE_FOLDER);

        admin.ProfileImageName = imageName;
        await UpdateAsync(admin);

        return await _fileService.GetImageResultAsync(imageName, IMAGE_FOLDER);
    }
}