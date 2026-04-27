using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class ExperienceValidator : AbstractValidator<ExperienceRequestModel>
    {
        public ExperienceValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.CompanyName))
                .MaximumLength(200)
                .WithMessage(Messages.CompanyNameMaxLength);

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Role))
                .MaximumLength(150)
                .WithMessage(Messages.RoleMaxLength);

            RuleFor(x => x.FromDate)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.FromDate));
        }
    }
}
