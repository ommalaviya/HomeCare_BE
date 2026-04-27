using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpRequestModel>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmail);

            RuleFor(x => x.Otp)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Otp))
                .Length(4, 10)
                .WithMessage(Messages.InvalidOtpLength);
        }
    }
}