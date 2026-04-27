using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Repositories;
using Shared.HomeCare.Services;
using System.Security.Claims;
using Shared.HomeCare.Hubs;

namespace Shared.HomeCare.Extensions
{
    public static class ConfigurationExtension
    {
        public static void RegisterSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();

        }

        public static void RegisterSharedRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddHttpContext(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(s =>
                s.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? new ClaimsPrincipal());
        }

        public static void RegisterSharedSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<IBookingNotificationService, BookingNotificationService>();
        }

        public static void RegisterBookingNotificationService(this IServiceCollection services, string adminBaseUrl)
        {
            services.AddHttpClient("AdminApiInternal", client =>
            {
                client.BaseAddress = new Uri(adminBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);
            });
 
            services.AddScoped<IBookingNotificationService, HttpBookingNotificationService>();
        }

        public static void MapBookingHub(this WebApplication app)
        {
            app.MapHub<BookingHub>("/hubs/booking");
        }

        // UPDATED — serves any subfolder dynamically
        public static void UseImageStaticFiles(this WebApplication app, string subFolder)
        {
            using var scope = app.Services.CreateScope();
            var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

            var folderPath = Path.Combine(fileService.BaseImagePath, subFolder);
            Directory.CreateDirectory(folderPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(folderPath),
                RequestPath = $"/resources/{subFolder}"
            });
        }
    }
}
