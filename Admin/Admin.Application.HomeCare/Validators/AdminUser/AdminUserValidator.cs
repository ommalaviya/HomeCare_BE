using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.AdminUser;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators.AdminUser
{
    public class CreateAdminUserValidator : AbstractValidator<CreateAdminUserRequestModel>
    {
        public CreateAdminUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Name"))
                .MaximumLength(100).WithMessage(string.Format(Messages.MaxLength, "Name", 100));

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Mobile Number"))
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage(Messages.InvalidMobileNumber);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Email"))
                .EmailAddress().WithMessage(Messages.InvalidEmailFormat)
                .MaximumLength(255).WithMessage(string.Format(Messages.EmailMaxLength, 255));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(string.Format(Messages.Required, Messages.Password))
                .MinimumLength(8).WithMessage(string.Format(Messages.MinLength, Messages.Password, 8))
                .MaximumLength(100).WithMessage(string.Format(Messages.MaxLength, Messages.Password, 100))
                .Matches("[A-Z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.UppercaseLetter))
                .Matches("[a-z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.LowercaseLetter))
                .Matches("[0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.Digit))
                .Matches("[^a-zA-Z0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.SpecialCharacter));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Confirm Password"))
                .Equal(x => x.Password).WithMessage(Messages.PasswordsMustMatch);
        }
    }

    public class UpdateAdminUserValidator : AbstractValidator<UpdateAdminUserRequestModel>
    {
        public UpdateAdminUserValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(string.Format(Messages.GreaterThanZero, "Id"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Name"))
                .MaximumLength(100).WithMessage(string.Format(Messages.MaxLength, "Name", 100));

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Mobile Number"))
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage(Messages.InvalidMobileNumber);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Email"))
                .EmailAddress().WithMessage(Messages.InvalidEmailFormat)
                .MaximumLength(255).WithMessage(string.Format(Messages.EmailMaxLength, 255));


            When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage(string.Format(Messages.MinLength, Messages.Password, 8))
                    .MaximumLength(100).WithMessage(string.Format(Messages.MaxLength, Messages.Password, 100))
                    .Matches("[A-Z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.UppercaseLetter))
                    .Matches("[a-z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.LowercaseLetter))
                    .Matches("[0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.Digit))
                    .Matches("[^a-zA-Z0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.SpecialCharacter));

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage(string.Format(Messages.Required, "Confirm Password"))
                    .Equal(x => x.Password).WithMessage(Messages.PasswordsMustMatch);
            });
        }
    }

    public class FilterAdminUserValidator : AbstractValidator<FilterAdminUserRequestModel>
    {
        public FilterAdminUserValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .When(x => x.PageNumber != 0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.PageNumber));

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .When(x => x.PageSize != 0)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.PageSize));
        }
    }

    public class ChangeAdminUserPasswordValidator : AbstractValidator<ChangeAdminUserPasswordRequestModel>
    {
        public ChangeAdminUserPasswordValidator()
        {
            RuleFor(x => x.TargetAdminId)
                .GreaterThan(0).WithMessage(string.Format(Messages.GreaterThanZero, "Admin Id"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(string.Format(Messages.Required, Messages.Password))
                .MinimumLength(8).WithMessage(string.Format(Messages.MinLength, Messages.Password, 8))
                .MaximumLength(100).WithMessage(string.Format(Messages.MaxLength, Messages.Password, 100))
                .Matches("[A-Z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.UppercaseLetter))
                .Matches("[a-z]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.LowercaseLetter))
                .Matches("[0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.Digit))
                .Matches("[^a-zA-Z0-9]").WithMessage(string.Format(Messages.PasswordMustContain, Messages.SpecialCharacter));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(string.Format(Messages.Required, "Confirm Password"))
                .Equal(x => x.Password).WithMessage(Messages.PasswordsMustMatch);
        }
    }
}