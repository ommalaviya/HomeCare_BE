using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Validators;
using Shared.HomeCare.Entities;
using System.Net;

namespace Public.Api.HomeCare.Extensions
{
    public static class ValidationConfiguration
    {
        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<SendOtpValidator>();

            // Return ApiResponse format on validation failure
            // so frontend reads err?.error?.message consistently
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstError = context.ModelState
                        .Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault() ?? "Validation failed.";

                    var response = new ApiResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = firstError,
                        Data = null!
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}