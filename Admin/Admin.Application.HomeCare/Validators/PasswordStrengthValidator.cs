using FluentValidation;

namespace Shared.HomeCare.Validators
{
    public class PasswordStrengthValidator<T> : AbstractValidator<string>
    {
        public PasswordStrengthValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain at least one special character");
        }
    }
}