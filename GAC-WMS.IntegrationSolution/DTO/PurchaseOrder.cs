using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.DTO
{
    public class PurchaseOrder
    {
        public class PurchaseOrderDto
        {
            public string OrderId { get; set; }
            public DateTime ProcessingDate { get; set; }
            public string CustomerIdentifier { get; set; }
            public List<PurchaseOrderItemDto> Items { get; set; }
        }

        public class PurchaseOrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

     

    }
}
