using Public.Api.HomeCare.BackgroundServices;
using Public.Application.HomeCare.Interfaces;
using Public.Application.HomeCare.Services;
using Public.Domain.HomeCare.Interface;
using Public.Infrastructure.HomeCare.Repositories;
using Shared.HomeCare.Extensions;
using Shared.HomeCare.Resources;
using Stripe;

namespace Public.Api.HomeCare.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContext();
            services.RegisterSharedServices();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IServicePartnerService, ServicePartnerService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ISupportTicketService, SupportTicketService>();
            services.AddScoped<IServiceListService, ServiceListService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddHostedService<BookingCompletionService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IServiceDetailService, ServiceDetailService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<PaymentIntentService>();

            var adminBaseUrl = configuration["AdminApi:BaseUrl"]
                ?? throw new InvalidOperationException(Messages.BaseURLConfiguration);
            services.RegisterBookingNotificationService(adminBaseUrl);
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.RegisterSharedRepositories();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IServicePartnerRepository, ServicePartnerRepository>();
            services.AddScoped<IServicePartnerEducationRepository, ServicePartnerEducationRepository>();
            services.AddScoped<IServicePartnerExperienceRepository, ServicePartnerExperienceRepository>();
            services.AddScoped<IServicePartnerSkillRepository, ServicePartnerSkillRepository>();
            services.AddScoped<IServicePartnerServiceOfferedRepository, ServicePartnerServiceOfferedRepository>();
            services.AddScoped<IServicePartnerLanguageRepository, ServicePartnerLanguageRepository>();
            services.AddScoped<IServicePartnerAttachmentRepository, ServicePartnerAttachmentRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
        }

        public static void RegisterNominatimClient(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["Nominatim:BaseUrl"] ?? "https://nominatim.openstreetmap.org";
            var userAgent = configuration["Nominatim:UserAgent"] ?? "HomeCareApp/1.0 (contact@homecare.dev)";
            var timeoutSeconds = int.Parse(configuration["Nominatim:TimeoutSeconds"] ?? "10");

            services.AddHttpClient("Nominatim", client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });
        }
    }
}