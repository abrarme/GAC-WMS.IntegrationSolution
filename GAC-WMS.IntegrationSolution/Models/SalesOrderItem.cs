using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class SalesOrderItem
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
