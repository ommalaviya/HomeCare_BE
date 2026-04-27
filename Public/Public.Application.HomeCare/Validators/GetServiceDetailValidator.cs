using FluentValidation;
using Shared.HomeCare.Resources;
namespace Public.Application.HomeCare.Validators
{
    public class GetServiceDetailValidator : AbstractValidator<int>
    {
        public GetServiceDetailValidator()
        {
            RuleFor(id => id)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.GreaterThanZero, Messages.ServiceId));
        }
    }
}