using FluentValidation;
using GAC_WMS.IntegrationSolution.DTO;

namespace GAC_WMS.IntegrationSolution.Validator
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Product code is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description too long.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Dimensions)
                .MaximumLength(200).WithMessage("Dimensions too long.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dimensions));
        }
    }
}
