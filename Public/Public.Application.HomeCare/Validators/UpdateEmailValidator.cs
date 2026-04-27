using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class UpdateEmailValidator : AbstractValidator<UpdateEmailRequestModel>
    {
        public UpdateEmailValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmail)
                .MaximumLength(255)
                .WithMessage(Messages.EmailMaxLength);

            RuleFor(x => x.Otp)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Otp))
                .Length(4, 10)
                .WithMessage(Messages.InvalidOtpLength);
        }
    }
}