using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.HomeCare.Validators
{
    public static class SharedValidationExtensions
    {

        public static IServiceCollection AddSharedValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstError = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .FirstOrDefault() ?? "Validation failed.";

                    return new ObjectResult(new { message = firstError })
                    {
                        StatusCode = 400
                    };
                };
            });

            return services;
        }
    }
}