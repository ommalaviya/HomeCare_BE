using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class FilterServicePartnerRequestValidator : AbstractValidator<FilterServicePartnerRequestModel>
    {
        public FilterServicePartnerRequestValidator()
        {
            RuleFor(x => x.JobsCompletedMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.JobsCompletedMin.HasValue)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.SupportTicket));

            RuleFor(x => x.JobsCompletedMax)
                .GreaterThanOrEqualTo(0)
                .When(x => x.JobsCompletedMax.HasValue)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.SupportTicket));

            RuleFor(x => x.JobsCompletedMax)
                .GreaterThanOrEqualTo(x => x.JobsCompletedMin)
                .When(x => x.JobsCompletedMin.HasValue && x.JobsCompletedMax.HasValue)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.SupportTicket));
        }
    }
}