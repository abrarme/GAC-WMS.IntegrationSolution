using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        public string OrderId { get; set; }

        [Required]
        public DateTime ProcessingDate { get; set; }

        [Required]
        public string CustomerIdentifier { get; set; }

        public Customer Customer { get; set; }


        public ICollection<PurchaseOrderItem> Items { get; set; }
    }
}
