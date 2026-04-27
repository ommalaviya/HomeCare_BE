using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmailFormat);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Password));
        }
    }
}