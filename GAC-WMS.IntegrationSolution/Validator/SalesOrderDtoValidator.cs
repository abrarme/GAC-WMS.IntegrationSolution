using FluentValidation;
using static GAC_WMS.IntegrationSolution.DTO.SalesOrder;

namespace GAC_WMS.IntegrationSolution.Validator
{

    public class SalesOrderDtoValidator : AbstractValidator<SalesOrderDto>
    {
        public SalesOrderDtoValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.ProcessingDate).NotEmpty();
            RuleFor(x => x.CustomerIdentifier).NotEmpty();
            RuleFor(x => x.ShipmentAddress)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).SetValidator(new SalesOrderItemDtoValidator());
        }
    }

    public class SalesOrderItemDtoValidator : AbstractValidator<SalesOrderItemDto>
    {
        public SalesOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
