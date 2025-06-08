using FluentValidation;
using GAC_WMS.IntegrationSolution.DTO;

namespace GAC_WMS.IntegrationSolution.Validator
{

    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
        {
            RuleFor(x => x.CustomerIdentifier)
                .NotEmpty().WithMessage("Customer identifier is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

            RuleFor(x => x.Contact)
                .MaximumLength(200).WithMessage("Contact cannot exceed 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");
        }
    }   

}
