using Admin.Application.HomeCare.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Shared.HomeCare.Validators;

namespace Admin.Api.HomeCare.Extensions
{
    public static class ValidationConfiguration
    {
        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            // Enable automatic model validation
            services.AddFluentValidationAutoValidation();

            // Register Validations
            services.AddValidatorsFromAssemblyContaining<CreateServiceTypeValidator>();
            services.AddSharedValidationResponse(); 

            return services;
        }
    }
}