using FluentValidation;
using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Shared.HomeCare.Resources;

namespace Admin.Application.HomeCare.Validators
{
    public class CreateOfferValidator : AbstractValidator<CreateOfferRequestModel>
    {
        public CreateOfferValidator()
        {
            RuleFor(x => x.CouponCode)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.CouponCode))
                .MaximumLength(100)
                .WithMessage(string.Format(Messages.MaxLength, Messages.CouponCode,100));

            RuleFor(x => x.CouponDescription)
                .MaximumLength(255)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Description,255));

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(1, 99)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Offer));
        }
    }

    public class UpdateOfferValidator : AbstractValidator<UpdateOfferRequestModel>
    {
        public UpdateOfferValidator()
        {
            RuleFor(x => x.CouponCode)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.CouponCode))
                .MaximumLength(100)
                .WithMessage(string.Format(Messages.MaxLength, Messages.CouponCode,100));

            RuleFor(x => x.CouponDescription)
                .MaximumLength(255)
                .WithMessage(string.Format(Messages.MaxLength, Messages.Description,255));

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(1, 99)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Offer));
        }
    }

    public class FilterOfferValidator : AbstractValidator<FilterOfferRequestModel>
    {
        public FilterOfferValidator()
        {
            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 99)
                .When(x => x.DiscountPercentage.HasValue)
                .WithMessage(string.Format(Messages.RangeBetween, Messages.Offer));

            RuleFor(x => x)
                .Must(x => x.AppliedCountMin <= x.AppliedCountMax)
                .When(x => x.AppliedCountMin.HasValue && x.AppliedCountMax.HasValue)
                .WithMessage(string.Format(Messages.RangeInvalid, Messages.Offer));

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
}