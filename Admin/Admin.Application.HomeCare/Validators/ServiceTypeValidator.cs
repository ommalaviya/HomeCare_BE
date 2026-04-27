using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.ServiceTypes;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class CreateServiceTypeValidator : AbstractValidator<CreateServiceTypeRequestModel>
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImageSizeMb = 5;

        public CreateServiceTypeValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ServiceType))
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.ServiceType, 150));

            RuleFor(x => x.Image)
                .Must(image => image.Length <= MaxImageSizeBytes)
                .WithMessage(string.Format(Messages.FileSizeExceeds, "Image", MaxImageSizeMb))
                .When(x => x.Image != null);
        }
    }

    public class UpdateServiceTypeValidator : AbstractValidator<UpdateServiceTypeRequestModel>
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImageSizeMb = 5;

        public UpdateServiceTypeValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ServiceType))
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.ServiceType, 150));

            RuleFor(x => x.Image)
                .Must(image => image.Length <= MaxImageSizeBytes)
                .WithMessage(string.Format(Messages.FileSizeExceeds, "Image", MaxImageSizeMb))
                .When(x => x.Image != null);
        }
    }
}