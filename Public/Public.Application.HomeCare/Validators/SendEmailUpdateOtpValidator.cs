using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class SendEmailUpdateOtpValidator : AbstractValidator<SendEmailUpdateOtpRequestModel>
    {
        public SendEmailUpdateOtpValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmail)
                .MaximumLength(255)
                .WithMessage(Messages.EmailMaxLength);
        }
    }
}