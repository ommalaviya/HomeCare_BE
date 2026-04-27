using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class EducationValidator : AbstractValidator<EducationRequestModel>
    {
        public EducationValidator()
        {
            RuleFor(x => x.SchoolCollege)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.SchoolCollege))
                .MaximumLength(200)
                .WithMessage(Messages.SchoolCollegeMaxLength);

            RuleFor(x => x.PassingYear)
                .GreaterThan(1950)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage(Messages.InvalidPassingYear);
        }
    }
}
