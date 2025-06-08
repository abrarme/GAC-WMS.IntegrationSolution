using FluentValidation;
using static GAC_WMS.IntegrationSolution.DTO.PurchaseOrder;

namespace GAC_WMS.IntegrationSolution.Validator
{
    public class PurchaseOrderDtoValidator : AbstractValidator<PurchaseOrderDto>
    {
        public PurchaseOrderDtoValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.ProcessingDate).NotEmpty();
            RuleFor(x => x.CustomerIdentifier).NotEmpty();
            RuleForEach(x => x.Items).SetValidator(new PurchaseOrderItemDtoValidator());
        }
    }

    public class PurchaseOrderItemDtoValidator : AbstractValidator<PurchaseOrderItemDto>
    {
        public PurchaseOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
