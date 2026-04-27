using AutoMapper;
using Public.Domain.HomeCare.DataModels.Request.Address;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.DataModels.Request.Payment;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Public.Domain.HomeCare.DataModels.Response.Address;
using Public.Domain.HomeCare.DataModels.Response.Booking;
using Public.Domain.HomeCare.DataModels.Response.Home;
using Public.Domain.HomeCare.DataModels.Response.Payment;
using Public.Domain.HomeCare.DataModels.Response.Offer;
using Public.Domain.HomeCare.DataModels.Response.ServiceList;
using Public.Domain.HomeCare.DataModels.Response.ServicePartners;
using Public.Domain.HomeCare.DataModels.Response.ServiceType;
using Public.Domain.HomeCare.DataModels.Response.SupportTicket;
using Public.Domain.HomeCare.DataModels.Response.Users;
using Shared.HomeCare.Entities;
using Public.Domain.HomeCare.DataModels.Request.SupportTicket;
using Stripe;

namespace Public.Application.HomeCare.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, GetUserResponseModel>().ReverseMap();
            CreateMap<CreateOrUpdateUserRequestModel, User>();

            // Address
            CreateMap<UserAddress, AddressResponseModel>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId.ToString("D3")));
            CreateMap<CreateAddressRequestModel, UserAddress>();
            CreateMap<UpdateAddressRequestModel, UserAddress>();

            // Home
            CreateMap<ServiceWithBookingCount, ServiceResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.Service.Images != null && src.Service.Images.Any()
                    ? $"/resources/Services/{src.Service.Images.First().ImageName}"
                    : null))
                .ForMember(dest => dest.ServiceTypeId, opt => opt.MapFrom(src =>
                    src.Service.SubCategory != null && src.Service.SubCategory.Category != null
                    ? src.Service.SubCategory.Category.ServiceTypeId
                    : 0))
                .ForMember(dest => dest.SelectedCategoryName, opt => opt.MapFrom(src =>
                    src.Service.SubCategory != null && src.Service.SubCategory.Category != null
                    ? src.Service.SubCategory.Category.CategoryName
                    : string.Empty))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.TotalBookings))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Service.IsAvailable));

            CreateMap<ServicesOfSubCategory, ServiceResponseModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any(i => !i.IsDeleted)
                        ? $"/resources/Services/{src.Images.First(i => !i.IsDeleted).ImageName}"
                        : null))
                .ForMember(dest => dest.ServiceTypeId, opt => opt.MapFrom(src =>
                    src.SubCategory != null && src.SubCategory.Category != null
                        ? src.SubCategory.Category.ServiceTypeId
                        : 0))
                .ForMember(dest => dest.SelectedCategoryName, opt => opt.MapFrom(src =>
                    src.SubCategory != null && src.SubCategory.Category != null
                        ? src.SubCategory.Category.CategoryName
                        : string.Empty))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(_ => 0));

            CreateMap<ServiceTypes, ServiceTypeResponseModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.ImageName != null
                        ? $"/resources/ServiceType/{src.ImageName}"
                        : null));

            // Service Detail
            CreateMap<ServicesOfSubCategory, ServiceDetailResponseModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.SubCategory != null
                        && src.SubCategory.Category != null
                        ? src.SubCategory.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.ServiceTypeId,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        && src.SubCategory.Category != null
                        && src.SubCategory.Category.ServiceTypes != null
                        ? src.SubCategory.Category.ServiceTypes.Id : 0))
                .ForMember(dest => dest.ServiceTypeName,
                    opt => opt.MapFrom(src => src.SubCategory != null
                        && src.SubCategory.Category != null
                        && src.SubCategory.Category.ServiceTypes != null
                        ? src.SubCategory.Category.ServiceTypes.ServiceName : string.Empty))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images
                        .Select(img => $"/resources/Services/{img.ImageName}").ToList()))
                .ForMember(dest => dest.Inclusions,
                    opt => opt.MapFrom(src => src.ServiceFilters
                        .Where(f => f.Type == Shared.HomeCare.Enums.ServiceInclusionExclusionType.Inclusion)
                        .Select(f => f.Item).ToList()))
                .ForMember(dest => dest.Exclusions,
                    opt => opt.MapFrom(src => src.ServiceFilters
                        .Where(f => f.Type == Shared.HomeCare.Enums.ServiceInclusionExclusionType.Exclusion)
                        .Select(f => f.Item).ToList()))
                .ForMember(dest => dest.RelatedServices, opt => opt.Ignore());

            // Service Partner
            CreateMap<ApplyServicePartnerRequestModel, ServicePartner>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
                    DateTime.SpecifyKind(src.DateOfBirth, DateTimeKind.Utc)))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationStatus, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
                .ForMember(dest => dest.TotalJobsCompleted, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ServicePartner, ApplyServicePartnerResponseModel>();

            // Education
            CreateMap<EducationRequestModel, ServicePartnerEducation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePartnerId, opt => opt.Ignore());

            // Experience
            CreateMap<ExperienceRequestModel, ServicePartnerExperience>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePartnerId, opt => opt.Ignore())
                .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src =>
                    DateTime.SpecifyKind(src.FromDate, DateTimeKind.Utc)))
                .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src =>
                    src.ToDate.HasValue ? DateTime.SpecifyKind(src.ToDate.Value, DateTimeKind.Utc) : (DateTime?)null));

            // Language
            CreateMap<LanguageRequestModel, ServicePartnerLanguage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePartnerId, opt => opt.Ignore());
            CreateMap<Language, GetLanguageResponseModel>();

            // Attachment
            CreateMap<AttachmentRequestModel, ServicePartnerAttachment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServicePartnerId, opt => opt.Ignore());

            CreateMap<ServiceTypeWithBookingCountDto, ServiceTypeBookingResponseModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.ImageName != null
                        ? $"/resources/ServiceType/{src.ImageName}"
                        : null))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.TotalBookings));

            CreateMap<(int TotalUsers, int TotalServices), CountResponseModel>()
                .ForMember(dest => dest.TotalUsers, opt => opt.MapFrom(src => src.TotalUsers))
                .ForMember(dest => dest.TotalServices, opt => opt.MapFrom(src => src.TotalServices));

            // Booking (Checkout)
            CreateMap<CreateBookingRequestModel, Booking>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => (Shared.HomeCare.Enums.PaymentMethod)src.PaymentMethod))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.BookingAmount, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedPartnerId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedPartner, opt => opt.Ignore())
                .ForMember(dest => dest.DurationInMinutes, opt => opt.Ignore())
                .ForMember(dest => dest.Transaction, opt => opt.Ignore())
                .ForMember(dest => dest.CouponUsage, opt => opt.Ignore());

            CreateMap<UserAddress, BookingAddressModel>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId));

            CreateMap<CreateBookingRequestModel, SlotAvailabilityRequestModel>()
                .ForMember(dest => dest.KnownDurationMinutes, opt => opt.Ignore());

            CreateMap<CreateTransactionIntentRequest, SlotAvailabilityRequestModel>()
                .ForMember(dest => dest.KnownDurationMinutes, opt => opt.Ignore());

            CreateMap<Booking, BookingResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.DurationInMinutes))
                .ForMember(dest => dest.AssignedPartner, opt => opt.MapFrom(src => src.AssignedPartner));

            CreateMap<ServicePartner, AssignedPartnerModel>();

            CreateMap<Booking, Transaction>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionAmount, opt => opt.MapFrom(src => src.BookingAmount))
                .ForMember(dest => dest.TransactionDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.StripePaymentIntentId, opt => opt.Ignore())
                .ForMember(dest => dest.Booking, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<PaymentIntent, TransactionIntentResponse>()
                .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret))
                .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Amount, opt => opt.Ignore());

            // Offer
            CreateMap<Offer, GetOfferResponseModel>();

            // ServiceList
            CreateMap<SubCategory, SubCategorySlimResponseModel>()
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Category, CategoryWithSubCategoriesResponseModel>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src =>
                    src.SubCategories.Where(sc => !sc.IsDeleted && sc.IsActive).ToList()));

            CreateMap<ServicesOfSubCategory, ServiceListItemResponseModel>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any(i => !i.IsDeleted)
                        ? $"/resources/Services/{src.Images.First(i => !i.IsDeleted).ImageName}"
                        : null));

            CreateMap<ServicesOfSubCategory, ServiceSearchResponseModel>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any(i => !i.IsDeleted)
                        ? $"/resources/Services/{src.Images.First(i => !i.IsDeleted).ImageName}"
                        : null));

            // Support Ticket
            CreateMap<CreateSupportTicketRequestModel, SupportTicket>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore());

            CreateMap<SupportTicket, GetSupportTicketResponseModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));

            CreateMap<UserAddress, MyBookingAddressModel>();

            CreateMap<ServicePartner, MyBookingPartnerModel>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<Booking, MyBookingResponseModel>()
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.ServiceTypeName,
                    opt => opt.MapFrom(src => src.ServiceType != null ? src.ServiceType.ServiceName : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.AssignedPartner, opt => opt.MapFrom(src => src.AssignedPartner))
                .AfterMap((src, dest) =>
                {
                    if (dest.AssignedPartner != null)
                        dest.AssignedPartner.Role = src.ServiceType?.ServiceName ?? string.Empty;
                });
        }
    }
}