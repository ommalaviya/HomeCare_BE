using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Admin;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators.Profile
{
    public class UpdateAdminPasswordValidator : AbstractValidator<UpdateAdminPasswordRequest>
    {
        public UpdateAdminPasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Password));

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Password))
                .MinimumLength(8)
                .WithMessage(string.Format(Messages.MinLength, Messages.Password, 8))
                .MaximumLength(100)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Password, 100))
                .Matches("[A-Z]")
                .WithMessage(string.Format(Messages.PasswordMustContain, Messages.UppercaseLetter))
                .Matches("[a-z]")
                .WithMessage(string.Format(Messages.PasswordMustContain, Messages.LowercaseLetter))
                .Matches("[0-9]")
                .WithMessage(string.Format(Messages.PasswordMustContain, Messages.Digit))
                .Matches("[^a-zA-Z0-9]")
                .WithMessage(string.Format(Messages.PasswordMustContain, Messages.SpecialCharacter));

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage(Messages.NewPasswordSameAsOld)
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));
        }
    }
}
