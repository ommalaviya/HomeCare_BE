using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Category;
using Shared.HomeCare.Resources;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestModel>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage(string.Format(Messages.Required, Messages.Category))
            .MaximumLength(150)
            .WithMessage(string.Format(Messages.MaxLength, Messages.Category, 150));
    }
}