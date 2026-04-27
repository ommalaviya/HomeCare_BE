using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Public.Domain.HomeCare.DataModels.Response.ServicePartners;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Constants;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Public.Application.HomeCare.Services
{
    public class ServicePartnerService(
        IServicePartnerRepository servicePartnerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IFileService fileService,
        IServicePartnerEducationRepository educationRepository,
        IServicePartnerExperienceRepository experienceRepository,
        IServicePartnerSkillRepository skillRepository,
        IServicePartnerServiceOfferedRepository serviceOfferedRepository,
        IServicePartnerLanguageRepository languageRepository,
        IServicePartnerAttachmentRepository attachmentRepository)
        : GenericService<ServicePartner>(servicePartnerRepository, unitOfWork, mapper, principal), IServicePartnerService
    {
        public async Task<ApplyServicePartnerResponseModel> ApplyAsync(ApplyServicePartnerRequestModel request)
        {
            if (await CheckDuplicateAsync(x => x.Email == request.Email && !x.IsDeleted))
                throw new DuplicateRecordException(Messages.EmailAlreadyExists);

            if (await CheckDuplicateAsync(x => x.MobileNumber == request.MobileNumber && !x.IsDeleted))
                throw new DuplicateRecordException(Messages.MobileAlreadyExists);

            var servicePartner = Map<ServicePartner, ApplyServicePartnerRequestModel>(request);
            servicePartner.VerificationStatus = VerificationStatus.Unverified;
            await AddAsync(servicePartner);

            var educations = request.Educations.Select(edu =>
            {
                var e = Map<ServicePartnerEducation, EducationRequestModel>(edu);
                e.ServicePartnerId = servicePartner.Id;
                e.CreatedBy = CurrentUserId;
                return e;
            }).ToList();
            await educationRepository.AddRangeAsync(educations);

            var experiences = request.Experiences.Select(exp =>
            {
                var e = Map<ServicePartnerExperience, ExperienceRequestModel>(exp);
                e.ServicePartnerId = servicePartner.Id;
                e.CreatedBy = CurrentUserId;
                return e;
            }).ToList();
            await experienceRepository.AddRangeAsync(experiences);

            var skills = request.SkillCategoryIds.Select(categoryId => new ServicePartnerSkill
            {
                ServicePartnerId = servicePartner.Id,
                CategoryId = categoryId,
                CreatedBy = CurrentUserId
            }).ToList();
            await skillRepository.AddRangeAsync(skills);

            var servicesOffered = request.ServiceSubCategoryIds.Select(subCategoryId => new ServicePartnerServiceOffered
            {
                ServicePartnerId = servicePartner.Id,
                SubCategoryId = subCategoryId,
                CreatedBy = CurrentUserId
            }).ToList();
            await serviceOfferedRepository.AddRangeAsync(servicesOffered);
          
            var languages = request.Languages.Select(lang =>
            {
                var l = Map<ServicePartnerLanguage, LanguageRequestModel>(lang);
                l.ServicePartnerId = servicePartner.Id;
                l.CreatedBy = CurrentUserId;
                return l;
            }).ToList();
            await languageRepository.AddRangeAsync(languages);

            var attachments = request.Attachments.Select(att =>
            {
                var a = Map<ServicePartnerAttachment, AttachmentRequestModel>(att);
                a.ServicePartnerId = servicePartner.Id;
                a.CreatedBy = CurrentUserId;
                return a;
            }).ToList();
            await attachmentRepository.AddRangeAsync(attachments);

            await unitOfWork.SaveChangesAsync();

            return ToResponseModel<ApplyServicePartnerResponseModel>(servicePartner);
        }

        public async Task<string> UploadProfileImageAsync(IFormFile file)
        {
            return await fileService.SaveImageAsync(file, SystemConstants.FolderNames.ServicePartnerImages);
        }

        public async Task<FileContentHttpResult> GetProfileImageAsync(string id)
        {
            return await fileService.GetImageResultAsync(id, SystemConstants.FolderNames.ServicePartnerImages);
        }

        public async Task<UploadAttachmentResponseModel> UploadAttachmentAsync(IFormFile file, string? documentLabel)
        {
            var fileName = await fileService.SaveAttachmentAsync(file, string.Empty);

            return new UploadAttachmentResponseModel
            {
                FileUrl = fileName,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSizeKb = (int)(file.Length / 1024)
            };
        }
    }
}