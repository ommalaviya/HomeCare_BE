using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Address;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class UpdateAddressValidator : AbstractValidator<UpdateAddressRequestModel>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.HouseFlatNumber)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.HouseFlatNumber))
                .MaximumLength(100)
                .WithMessage(string.Format(Messages.MaxLength, Messages.HouseFlatNumber, 100));

            RuleFor(x => x.FullAddress)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Address));

            RuleFor(x => (double)x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(string.Format(Messages.InvalidCoordinate, Messages.Latitude, -90, 90));

            RuleFor(x => (double)x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(string.Format(Messages.InvalidCoordinate, Messages.Longitude, -180, 180));
        }
    }
}
