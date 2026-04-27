using AutoMapper;
using Admin.Domain.HomeCare.DataModels.Request.ServiceTypes;
using Admin.Domain.HomeCare.DataModels.Response.ServiceTypes;
using Shared.HomeCare.Entities;
using Admin.Domain.HomeCare.DataModels.Response.Category;
using Admin.Domain.HomeCare.DataModels.Request.Category;
using Admin.Domain.HomeCare.DataModels.Response.SubCategory;
using Admin.Domain.HomeCare.DataModels.Request.SubCategory;
using Admin.Domain.HomeCare.DataModels.Response.Auth;
using Admin.Domain.HomeCare.DataModels.Request.Admin;
using Admin.Domain.HomeCare.DataModels.Response.Admin;
using Admin.Domain.HomeCare.DataModels.Response.Services;
using Admin.Domain.HomeCare.DataModels.Request.Services;
using Shared.HomeCare.Enums;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Admin.Domain.HomeCare.DataModels.Response.SupportTicket;
using Admin.Domain.HomeCare.DataModels.Response.AdminUser;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.ServicePartner;
using Admin.Domain.HomeCare.DataModels.Response.Transaction;
using Admin.Domain.HomeCare.DataModels.Response.Booking;

namespace Admin.Application.HomeCare.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ── ServiceType ──────────────────────────────────────────────────────
            CreateMap<ServiceTypes, GetServiceTypeResponseModel>().ReverseMap();

            CreateMap<CreateServiceTypeRequestModel, ServiceTypes>()
                .ForMember(dest => dest.ImageName, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateServiceTypeRequestModel, ServiceTypes>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageName, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            // ── Category ─────────────────────────────────────────────────────────
            CreateMap<Category, GetCategoryResponseModel>()
                .ForMember(dest => dest.ServiceTypeName,
                    opt => opt.MapFrom(src => src.ServiceTypes.ServiceName));

            CreateMap<CreateCategoryRequestModel, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ── SubCategory ──────────────────────────────────────────────────────
            CreateMap<SubCategory, GetSubCategoryResponseModel>().ReverseMap();

            CreateMap<CreateSubCategoryRequestModel, SubCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ── Auth ─────────────────────────────────────────────────────────────
            CreateMap<AdminUser, LoginResponseModel>().ReverseMap();

            // ── Admin User Management ────────────────────────────────────────────
            CreateMap<AdminUser, GetAdminUserResponseModel>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.IsSuperAdmin ? "Super Admin" : "Admin"))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => !src.IsDeleted));

            // ── Admin Profile ────────────────────────────────────────────────────
            CreateMap<UpdateAdminContactRequest, AdminUser>()
                .ForMember(dest => dest.Email,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Email)))
                .ForMember(dest => dest.MobileNumber,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.MobileNumber)))
                .ForMember(dest => dest.Address,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Address)));

            CreateMap<AdminUser, AdminProfileResponse>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.IsSuperAdmin ? "Super Admin" : "Admin"))
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ProfileImageName)
                        ? null
                        : $"/admin/profile/image/{src.ProfileImageName}"));

            // ── Services (list) ──────────────────────────────────────────────────
            CreateMap<ServicesOfSubCategory, GetServicesListResponseModel>()
                .ForMember(dest => dest.SubCategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        ? src.SubCategory.SubCategoryName
                        : string.Empty));

            // ── ServicesImages → ServiceImageResponseModel ────────────────────────
            CreateMap<ServicesImages, ServiceImageResponseModel>()
               .ForMember(dest => dest.ImageUrl,
                   opt => opt.MapFrom(src => $"/resources/Services/{src.ImageName}"));

            // ── ServiceInclusionExclusion → ServiceFilterItemResponseModel ────────
            CreateMap<ServiceInclusionExclusion, ServiceFilterItemResponseModel>();

            // ── Services (detail) ────────────────────────────────────────────────
            CreateMap<ServicesOfSubCategory, GetServiceByIdResponseModel>()
                .ForMember(dest => dest.SubCategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        ? src.SubCategory.SubCategoryName
                        : string.Empty))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        ? src.SubCategory.Category.CategoryName
                        : string.Empty))
                .ForMember(dest => dest.ServiceTypeName,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        ? src.SubCategory.Category.ServiceTypes.ServiceName
                        : string.Empty))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.Images.Where(i => !i.IsDeleted).ToList()))
                .ForMember(dest => dest.InclusionItems,
                    opt => opt.MapFrom(src =>
                        src.ServiceFilters
                            .Where(f => f.Type == ServiceInclusionExclusionType.Inclusion && !f.IsDeleted)
                            .ToList()))
                .ForMember(dest => dest.ExclusionItems,
                    opt => opt.MapFrom(src =>
                        src.ServiceFilters
                            .Where(f => f.Type == ServiceInclusionExclusionType.Exclusion && !f.IsDeleted)
                            .ToList()));

            // ── Services (create, update) ─────────────────────────────────────────
            CreateMap<CreateServiceRequestModel, ServicesOfSubCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceFilters, opt => opt.Ignore());

            CreateMap<UpdateServiceRequestModel, ServicesOfSubCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceFilters, opt => opt.Ignore());

            // ── Offer ────────────────────────────────────────────────────────────
            CreateMap<CreateOfferRequestModel, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppliedCount, opt => opt.Ignore());

            CreateMap<UpdateOfferRequestModel, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppliedCount, opt => opt.Ignore());

            CreateMap<Offer, GetOfferResponseModel>();

            // ── Support Ticket ───────────────────────────────────────────────────
            CreateMap<SupportTicket, GetSupportTicketResponseModel>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Name));

            // ── Customer Management ──────────────────────────────────────────────
            CreateMap<User, GetCustomerResponseModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<CreateCustomerRequestModel, User>();

            CreateMap<ServicePartner, GetServicePartnerResponseModel>()
               .ForMember(dest => dest.Name,
                   opt => opt.MapFrom(src => src.FullName))
               .ForMember(dest => dest.Address,
                   opt => opt.MapFrom(src => src.ResidentialAddress))
               .ForMember(dest => dest.Job,
                   opt => opt.MapFrom(src => src.ServiceTypes != null
                       ? src.ServiceTypes.ServiceName
                       : string.Empty))
               .ForMember(dest => dest.JobsCompleted,
                   opt => opt.MapFrom(src => src.TotalJobsCompleted))
               .ForMember(dest => dest.Status,
                   opt => opt.MapFrom(src => src.Status.ToString()));

            // ── Service Partner — action response (approve / reject) ───────────────
            CreateMap<ServicePartner, ServicePartnerActionResponse>()
                .ForMember(dest => dest.VerificationStatus,
                    opt => opt.MapFrom(src => src.VerificationStatus.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Message, opt => opt.Ignore());

            // ── Service Partner — sub-entities ────────────────────────────────────
            CreateMap<ServicePartnerExperience, ServicePartnerExperienceResponse>()
                .ForMember(dest => dest.DurationYears, opt => opt.Ignore());

            CreateMap<ServicePartnerAttachment, ServicePartnerAttachmentResponse>()
                .ForMember(dest => dest.DocumentLabel,
                    opt => opt.MapFrom(src => src.DocumentLabel ?? src.FileName));

            CreateMap<ServicePartnerSkill, ServicePartnerSkillResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore());

            CreateMap<ServicePartnerServiceOffered, ServicePartnerServiceOfferedResponse>()
                .ForMember(dest => dest.SubCategoryName, opt => opt.Ignore());

            CreateMap<ServicePartnerLanguage, ServicePartnerLanguageResponse>()
                .ForMember(dest => dest.LanguageName, opt => opt.Ignore())
                .ForMember(dest => dest.Proficiency,
                    opt => opt.MapFrom(src => src.Proficiency.ToString()));

            // ── Service Partner — assigned service rows ───────────────────────────
            CreateMap<AssignedServiceRow, AssignedServiceResponse>();

            // ── Transaction ──────────────────────────────────────────────────────
            CreateMap<Transaction, GetTransactionResponseModel>()
                 .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
                 .ForMember(dest => dest.MobileNumber,
                     opt => opt.MapFrom(src => src.User != null ? src.User.MobileNumber : string.Empty))
                 .ForMember(dest => dest.ServiceName,
                     opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.PaymentMethod,
                     opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

            // ── Booking — child grid ─────────────────────────────────────────────
            CreateMap<Booking, BookingDetailResponse>()
                .ForMember(dest => dest.BookingId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.ServiceType,
                    opt => opt.MapFrom(src => src.ServiceType.ServiceName))
                .ForMember(dest => dest.AssignedExpertName,
                    opt => opt.MapFrom(src => src.AssignedPartner != null
                        ? src.AssignedPartner.FullName
                        : null))
                .ForMember(dest => dest.AssignedExpertImageUrl,
                    opt => opt.MapFrom(src => src.AssignedPartner != null
                        ? src.AssignedPartner.ProfileImageUrl
                        : null))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            // ── Available experts — Change Expert popup ───────────────────────────
            CreateMap<ServicePartner, AvailableExpertResponse>()
                .ForMember(dest => dest.PartnerId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsVerified,
                    opt => opt.MapFrom(src => true));
            CreateMap<Transaction, TransactionDetailResponseModel>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
                .ForMember(dest => dest.MobileNumber,
                    opt => opt.MapFrom(src => src.User != null ? src.User.MobileNumber : string.Empty))
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.PaymentType,
                    opt => opt.MapFrom(_ => "Service Payment"))
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

            CreateMap<Transaction, GetUserTransactionResponseModel>()
                .ForMember(dest => dest.ServiceName,
                     opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.PaymentMethod,
                     opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

            CreateMap<Booking, CustomerBookingDetailResponse>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType != null ? src.ServiceType.ServiceName : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address != null
                                                                    ? src.Address.FullAddress ?? src.Address.HouseFlatNumber
                                                                    : string.Empty))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AssignedExpertName, opt => opt.MapFrom(src => src.AssignedPartner != null ? src.AssignedPartner.FullName : null))
                .ForMember(dest => dest.AssignedExpertImageUrl, opt => opt.MapFrom(src => src.AssignedPartner != null ? src.AssignedPartner.ProfileImageUrl : null));
        }
    }
}