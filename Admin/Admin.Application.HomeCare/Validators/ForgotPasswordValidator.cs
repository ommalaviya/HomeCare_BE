using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequestModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmailFormat);
        }
    }
}