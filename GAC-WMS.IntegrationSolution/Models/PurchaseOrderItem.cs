using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
