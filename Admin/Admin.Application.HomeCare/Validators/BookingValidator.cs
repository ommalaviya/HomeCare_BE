using Admin.Domain.HomeCare.DataModels.Request.Booking;
using FluentValidation;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class ChangeExpertValidator : AbstractValidator<ChangeExpertRequestModel>
    {
        public ChangeExpertValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.BookingId));

            RuleFor(x => x.NewPartnerId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.NewPartnerId));
        }
    }

    public class CancelBookingValidator : AbstractValidator<CancelBookingRequestModel>
    {
        public CancelBookingValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.BookingId));

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.CancellationReason))
                .MaximumLength(500)
                .WithMessage(string.Format(Messages.MaxLength, Messages.CancellationReason, 500));
        }
    }
}