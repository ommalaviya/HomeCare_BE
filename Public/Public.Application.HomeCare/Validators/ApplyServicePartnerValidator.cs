using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Validators
{
    public class ApplyServicePartnerValidator : AbstractValidator<ApplyServicePartnerRequestModel>
    {
        public ApplyServicePartnerValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.FullName))
                .MaximumLength(150)
                .WithMessage(Messages.NameMaxLength);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.DateOfBirth));

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage(string.Format(Messages.Required, "Gender"));

            RuleFor(x => x.MobileNumber)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.MobileNumber))
                .MaximumLength(20)
                .WithMessage(Messages.MobileMaxLength);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Email))
                .EmailAddress()
                .WithMessage(Messages.InvalidEmail)
                .MaximumLength(255)
                .WithMessage(Messages.EmailMaxLength);

            RuleFor(x => x.ApplyingForTypeId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, Messages.ApplyingFor));

            RuleFor(x => x.PermanentAddress)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.PermanentAddress));

            RuleFor(x => x.ResidentialAddress)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ResidentialAddress));

            RuleFor(x => x.Educations)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Education));

            RuleForEach(x => x.Educations).SetValidator(new EducationValidator());

            RuleForEach(x => x.Experiences).SetValidator(new ExperienceValidator());

            RuleFor(x => x.SkillCategoryIds)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Skills));

            RuleFor(x => x.ServiceSubCategoryIds)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.ServicesOffered));

            RuleFor(x => x.Languages)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Language));

            RuleForEach(x => x.Languages).SetValidator(new LanguageRequestValidator());

            RuleFor(x => x.Attachments)
                .NotEmpty()
                .WithMessage(string.Format(Messages.Required, Messages.Attachments));

            RuleForEach(x => x.Attachments).SetValidator(new AttachmentValidator());
        }
    }
}
