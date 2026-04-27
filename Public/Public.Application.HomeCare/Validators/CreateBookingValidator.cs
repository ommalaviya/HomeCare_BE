using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingRequestModel>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.Services));

            RuleFor(x => x.ServiceTypeId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.ServiceType));

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.Address));

            RuleFor(x => x.BookingDate)
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(Messages.BookingDatePast);

            RuleFor(x => x.BookingTime)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.BookingTime))
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage(Messages.BookingTimeFormat);

            RuleFor(x => x.PaymentMethod)
                .InclusiveBetween(1, 2)
                .WithMessage(Messages.PaymentMethodInvalid);
        }
    }
}