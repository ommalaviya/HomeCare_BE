using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Admin;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators.Admin
{
    public class UpdateAdminContactValidator : AbstractValidator<UpdateAdminContactRequest>
    {
        public UpdateAdminContactValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Email)
                        || !string.IsNullOrWhiteSpace(x.MobileNumber)
                        || !string.IsNullOrWhiteSpace(x.Address))
                .WithMessage(string.Format(Messages.AtLeastOneFieldRequired,
                    $"{Messages.Email}, {Messages.MobileNumber}, or {Messages.Address}"));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(Messages.InvalidEmailFormat)
                .MaximumLength(150)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Email, 150))
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.MobileNumber)
                .Matches(@"^\+?[0-9]{7,15}$")
                .WithMessage(Messages.InvalidMobileNumber)
                .When(x => !string.IsNullOrWhiteSpace(x.MobileNumber));

            RuleFor(x => x.Address)
                .MaximumLength(250)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Address, 250))
                .When(x => !string.IsNullOrWhiteSpace(x.Address));
        }
    }
}