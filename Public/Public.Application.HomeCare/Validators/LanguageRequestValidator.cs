using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class LanguageRequestValidator : AbstractValidator<LanguageRequestModel>
    {
        public LanguageRequestValidator()
        {
            RuleFor(x => x.LanguageId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.Language));

            RuleFor(x => x.Proficiency)
                .IsInEnum()
                .WithMessage(string.Format(Messages.Required, Messages.Proficiency));
        }
    }
}
