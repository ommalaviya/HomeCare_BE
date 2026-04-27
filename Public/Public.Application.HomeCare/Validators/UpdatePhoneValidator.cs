using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class UpdatePhoneValidator : AbstractValidator<UpdatePhoneRequestModel>
    {
        public UpdatePhoneValidator()
        {
            RuleFor(x => x.MobileNumber)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.MobileNumber))
                .Matches(@"^\+?[0-9]{7,15}$")
                .WithMessage(Messages.InvalidMobileNumber);
        }
    }
}