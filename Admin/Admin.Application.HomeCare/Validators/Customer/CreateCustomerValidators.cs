using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators.Customer
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequestModel>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.FullName))
                .MaximumLength(150)
                .WithMessage(Messages.NameMaxLength);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmailFormat)
                .MaximumLength(255)
                .WithMessage(Messages.EmailMaxLength);

            RuleFor(x => x.MobileNumber)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.MobileNumber))
                .Matches(@"^\+?[0-9]{7,15}$")
                .WithMessage(Messages.InvalidMobileNumber)
                .MaximumLength(20)
                .WithMessage(Messages.MobileMaxLength);
        }
    }
}