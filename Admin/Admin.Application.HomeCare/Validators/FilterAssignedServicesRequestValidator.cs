using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators.ServicePartner
{
    public class FilterAssignedServicesRequestValidator : AbstractValidator<FilterAssignedServicesRequestModel>
    {
        public FilterAssignedServicesRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .When(x => x.PageNumber > 0)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.PageNumber));

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .When(x => x.PageSize > 0)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.PageSize));
        }
    }
}