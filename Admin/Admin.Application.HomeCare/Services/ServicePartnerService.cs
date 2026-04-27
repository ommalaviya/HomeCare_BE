using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Admin.Domain.HomeCare.DataModels.Response.ServicePartner;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Extensions;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class ServicePartnerService(
        IServicePartnerRepository servicePartnerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileService fileService,
        ClaimsPrincipal principal)
        : GenericService<ServicePartner>(servicePartnerRepository, unitOfWork, mapper, principal),
          IServicePartnerService
    {
        private const string AttachmentsSubFolder = "Attachments";
        public async Task<FilteredDataQueryResponseModel<GetServicePartnerResponseModel>> GetAllServicePartnersAsync(
    FilterServicePartnerRequestModel? filter = null)
        {
            filter ??= new FilterServicePartnerRequestModel();

            var result = await servicePartnerRepository.GetServicePartnersWithMetaAsync(filter);

            return new FilteredDataQueryResponseModel<GetServicePartnerResponseModel>
            {
                TotalRecords = result.TotalRecords,
                Records = result.Records,
                FilterMeta = result.FilterMeta
            };
        }
        public async Task<ServicePartnerDetailResponse> GetDetailAsync(int id)
        {
            var projection = await servicePartnerRepository.GetDetailProjectionAsync(id);
            return BuildDetailResponse(projection);
        }
        public async Task<IEnumerable<PartnerServiceResponseModel>> GetPartnerServicesAsync(int servicePartnerId)
        {
            _ = await FindOrThrowAsync(
                x => x.Id == servicePartnerId ,
                string.Format(Messages.NotFound, Messages.ServicePartner));

            return await servicePartnerRepository.GetServicesByPartnerTypeAsync(servicePartnerId);
        }
        public async Task<ServicePartnerActionResponse> ApproveAsync(int id)
        {
            var sp = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                Messages.AccountDeleted);

            if (sp.VerificationStatus == VerificationStatus.Verified)
                throw new InvalidOperationException(Messages.AlreadyApproved);

            sp.VerificationStatus = VerificationStatus.Verified;
            sp.Status = ServicePartnerStatus.Active;
            sp.VerifiedAt = DateTime.UtcNow;
            sp.VerifiedBy = CurrentUserId;
            sp.RejectionReason = null;

            await UpdateAsync(sp);

            var result = mapper.Map<ServicePartnerActionResponse>(sp);
            result.Message = string.Format(Messages.ApprovedSuccessfully, Messages.ServicePartner);
            return result;
        }
        public async Task<ServicePartnerActionResponse> RejectAsync(int id, RejectServicePartnerRequestModel request)
        {
            var sp = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                 Messages.AccountDeleted);

            if (sp.VerificationStatus == VerificationStatus.Unverified
                && sp.Status == ServicePartnerStatus.Rejected)
                throw new InvalidOperationException(Messages.AlreadyRejected);

            sp.VerificationStatus = VerificationStatus.Unverified;
            sp.Status = ServicePartnerStatus.Rejected;
            sp.RejectionReason = request.RejectionReason;

            await UpdateAsync(sp);

            var result = mapper.Map<ServicePartnerActionResponse>(sp);
            result.Message = string.Format(Messages.RejectedSuccessfully, Messages.ServicePartner);
            return result;
        }
        public async Task<DataQueryResponseModel<AssignedServiceResponse>> GetAssignedServicesAsync(
            int servicePartnerId,
            FilterAssignedServicesRequestModel filter)
        {
            var sp = await FindOrThrowAsync(
                x => x.Id == servicePartnerId ,
                string.Format(Messages.NotFound, Messages.ServicePartner));

            if (sp.VerificationStatus != VerificationStatus.Verified)
            {
                return new DataQueryResponseModel<AssignedServiceResponse>
                {
                    TotalRecords = 0,
                    Records = [],
                    Message = Messages.ServicePartnerNotVerified
                };
            }

            var (totalRecords, rows) =
                await servicePartnerRepository.GetAssignedBookingsAsync(servicePartnerId, filter);

            return new DataQueryResponseModel<AssignedServiceResponse>
            {
                TotalRecords = totalRecords,
                Records = mapper.Map<List<AssignedServiceResponse>>(rows.ToList())
            };
        }
        public async Task<bool> ToggleStatusAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                 Messages.AccountDeleted);

            // Only block when transitioning from Active to Inactive
            if (entity.Status == ServicePartnerStatus.Active)
            {
                int pendingCount = await servicePartnerRepository.GetPendingBookingCountAsync(id);
                if (pendingCount > 0)
                {
                    int otherActiveCount = await servicePartnerRepository
                        .GetOtherActivePartnerCountForServiceTypeAsync(id);

                    string message = otherActiveCount == 0
                        ? Messages.CannotDeactivatePartnerSoleAssigned
                        : Messages.CannotDeactivatePartnerChangeExpert;

                    throw new InvalidOperationException(message);
                }
            }

            entity.Status = entity.Status == ServicePartnerStatus.Active
                ? ServicePartnerStatus.Inactive
                : ServicePartnerStatus.Active;

            await UpdateAsync(entity);
            return true;
        }
        public async Task<bool> DeleteServicePartnerAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                Messages.AccountDeleted);

            int pendingCount = await servicePartnerRepository.GetPendingBookingCountAsync(id);
            if (pendingCount > 0)
            {
                int otherActiveCount = await servicePartnerRepository
                    .GetOtherActivePartnerCountForServiceTypeAsync(id);

                string message = otherActiveCount == 0
                    ? Messages.CannotDeletePartnerSoleAssigned
                    : Messages.CannotDeletePartnerChangeExpert;

                throw new InvalidOperationException(message);
            }

            entity.Status = ServicePartnerStatus.Inactive;
            await SoftDeleteAsync(entity);
            return true;
        }
        public async Task<FileContentHttpResult> DownloadAttachmentAsync(int servicePartnerId, int attachmentId)
        {
            _ = await FindOrThrowAsync(
                x => x.Id == servicePartnerId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.ServicePartner));

            var attachment = await servicePartnerRepository.GetAttachmentAsync(servicePartnerId, attachmentId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Attachments));

            if (string.IsNullOrWhiteSpace(attachment.FileUrl))
                throw new InvalidOperationException(Messages.FileUrl);

            var fileName = Path.GetFileName(attachment.FileUrl);
            return await fileService.GetFileResultAsync(fileName, string.Empty);
        }
        private ServicePartnerDetailResponse BuildDetailResponse(ServicePartnerDetailProjection p)
        {
            var sp = p.ServicePartner;

            var skills = mapper.Map<List<ServicePartnerSkillResponse>>(p.Skills);
            foreach (var s in skills)
                s.CategoryName = p.CategoryNames.GetValueOrDefault(s.CategoryId, string.Empty);

            var servicesOffered = mapper.Map<List<ServicePartnerServiceOfferedResponse>>(p.ServicesOffered);
            foreach (var s in servicesOffered)
                s.SubCategoryName = p.SubCategoryNames.GetValueOrDefault(s.SubCategoryId, string.Empty);

            var languagesSpoken = mapper.Map<List<ServicePartnerLanguageResponse>>(p.Languages);
            foreach (var lang in languagesSpoken)
                lang.LanguageName = p.LanguageNames.GetValueOrDefault(lang.LanguageId, string.Empty);

            var experiences = p.Experiences.Select(e =>
            {
                var exp = mapper.Map<ServicePartnerExperienceResponse>(e);
                exp.DurationYears = MonthsToYears(ComputeMonths(e.FromDate, e.ToDate));
                return exp;
            }).ToList();

            var attachments = mapper.Map<List<ServicePartnerAttachmentResponse>>(p.Attachments);

            var totalWorkExperience = MonthsToYears(
                p.Experiences.Sum(e => ComputeMonths(e.FromDate, e.ToDate)));

            return new ServicePartnerDetailResponse
            {
                Id = sp.Id,
                FullName = sp.FullName,
                MobileNumber = sp.MobileNumber,
                Email = sp.Email,
                ResidentialAddress = sp.ResidentialAddress,
                JobTitle = p.ServiceTypeName,
                TotalWorkExperienceYears = totalWorkExperience,
                VerificationStatus = sp.VerificationStatus.ToString(),
                Status = sp.Status.ToString(),
                ProfileImageUrl = sp.ProfileImageUrl,
                Skills = skills,
                ServicesOffered = servicesOffered,
                LanguagesSpoken = languagesSpoken,
                PreviousExperiences = experiences,
                Attachments = attachments
            };
        }
        private static int ComputeMonths(DateTime from, DateTime? to)
        {
            var end = to ?? DateTime.UtcNow;
            return (end.Year - from.Year) * 12 + (end.Month - from.Month);
        }
        private static decimal MonthsToYears(int totalMonths)
        {
            var years = totalMonths / 12;
            var months = totalMonths % 12;
            if (months == 12)
            {
                years += 1;
                months = 0;
            }
            return years + (months / 100m);
        }
    }
}