using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Services;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class CreateServiceValidator : AbstractValidator<CreateServiceRequestModel>
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImageSizeMb = 5;

        public CreateServiceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ServiceName))
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.ServiceName, 150));

            RuleFor(x => x.SubCategoryId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.SubCategory));

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Duration))
                .MaximumLength(3)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Duration, 1, 3));

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Price))
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.Price));

            RuleFor(x => x.Commission)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Commission))
                .InclusiveBetween(1, 99)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Commission, 1, 99));

            RuleForEach(x => x.Images)
                .Must(image => image.Length <= MaxImageSizeBytes)
                .WithMessage(string.Format(Messages.FileSizeExceeds, "Image", MaxImageSizeMb))
                .When(x => x.Images != null && x.Images.Count > 0);

             RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Description,255));    
        }
    }

    public class UpdateServiceValidator : AbstractValidator<UpdateServiceRequestModel>
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImageSizeMb = 5;

        public UpdateServiceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ServiceName))
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.ServiceName, 150));

            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.SubCategory));

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Duration))
                .MaximumLength(3)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Duration, 1, 3));

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Price))
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.Price));

            RuleFor(x => x.Commission)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Commission))
                .InclusiveBetween(1, 99)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Commission, 1, 99));

            RuleForEach(x => x.NewImages)
                .Must(image => image.Length <= MaxImageSizeBytes)
                .WithMessage(string.Format(Messages.FileSizeExceeds, "Image", MaxImageSizeMb))
                .When(x => x.NewImages != null && x.NewImages.Count > 0);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Description,255));  
        }
    }
}