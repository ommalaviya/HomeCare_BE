using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.SubCategory;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class CreateSubCategoryValidator : AbstractValidator<CreateSubCategoryRequestModel>
    {
        public CreateSubCategoryValidator()
        {
            RuleFor(x => x.SubCategoryName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.SubCategory))
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.SubCategory, 150));
        }
    }
}