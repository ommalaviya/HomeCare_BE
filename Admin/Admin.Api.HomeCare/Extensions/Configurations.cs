using System.Security.Claims;
using Admin.Application.HomeCare.Interfaces;
using Admin.Application.HomeCare.Services;
using Admin.Domain.HomeCare.Interface;
using Admin.Infrastructure.HomeCare.Repositories;
using Application.HomeCare.Interfaces;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Extensions;
using Shared.HomeCare.Services;
using Shared.Interfaces.Services;

namespace Admin.Api.HomeCare.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddHttpContext();
            services.RegisterSharedServices();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAdminProfileService, AdminProfileService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<ISupportTicketService, SupportTicketService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IServicePartnerService, ServicePartnerService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IGenericService<Transaction>, GenericService<Transaction>>();
            services.AddScoped<IBookingService, BookingService>();
        }


        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.RegisterSharedRepositories();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<IServicesImagesRepository, ServicesImagesRepository>();
            services.AddScoped<IServiceInclusionExclusionRepository, ServiceInclusionExclusionRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IServicePartnerRepository, ServicePartnerRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
        }
    }
}