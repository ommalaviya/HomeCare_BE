using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Shared.HomeCare.Resources;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequestModel>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(string.Format(Messages.Required, Messages.Token));

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

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(string.Format(Messages.Required, Messages.ConfirmPassword))
            .Equal(x => x.NewPassword)
            .WithMessage(Messages.PasswordsMustMatch);
    }
}
