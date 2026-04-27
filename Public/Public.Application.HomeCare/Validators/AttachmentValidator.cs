using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class AttachmentValidator : AbstractValidator<AttachmentRequestModel>
    {
        public AttachmentValidator()
        {
            RuleFor(x => x.FileUrl)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.FileUrl));

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.FileName));
        }
    }
}
