using Admin.Domain.HomeCare.DataModels.Request.SupportTicket;
using FluentValidation;
using Shared.HomeCare.Resources;

public class FilterSupportTicketValidator : AbstractValidator<FilterSupportTicketRequestModel>
{
    public FilterSupportTicketValidator()
    {
        RuleFor(x => x.UserName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.UserName))
            .WithMessage(string.Format(Messages.MaxLength, Messages.UserName,100));

        RuleFor(x => x.SubmittedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.SubmittedAt.HasValue)
            .WithMessage(string.Format(Messages.InvalidDate, Messages.SubmittedAt));

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