using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.Offer;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class CouponValidator : AbstractValidator<ValidateCouponRequestModel>
    {
        public CouponValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.Services));

            RuleFor(x => x.OfferId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.Offer));
        }
    }
}