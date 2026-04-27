using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Address;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class ReverseGeocodeValidator : AbstractValidator<ReverseGeocodeRequestModel>
    {
        public ReverseGeocodeValidator()
        {
            RuleFor(x => (double)x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(string.Format(Messages.InvalidCoordinate, Messages.Latitude, -90, 90));

            RuleFor(x => (double)x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(string.Format(Messages.InvalidCoordinate, Messages.Longitude, -180, 180));
        }
    }
}
