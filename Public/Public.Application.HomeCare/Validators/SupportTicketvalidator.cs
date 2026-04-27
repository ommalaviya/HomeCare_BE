using FluentValidation;
using Public.Domain.HomeCare.DataModels.Request.SupportTicket;
using Shared.HomeCare.Resources;

public class CreateSupportTicketValidator : AbstractValidator<CreateSupportTicketRequestModel>
{
    public CreateSupportTicketValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage(string.Format(Messages.Required, Messages.FirstName));

        RuleFor(x => x.ContactNumber)
            .NotEmpty()
            .Matches(@"^[6-9]\d{9}$") 
            .WithMessage(string.Format(Messages.InvalidMobileNumber));

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .WithMessage(string.Format(Messages.InvalidEmail));

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage(string.Format(Messages.MaxLength, Messages.Description,500));
    }
}