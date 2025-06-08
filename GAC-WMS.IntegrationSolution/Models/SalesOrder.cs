using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }
        [Required]
        public string OrderId { get; set; }
        [Required]
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        [Required]
        public string ShipmentAddress { get; set; }
        public List<SalesOrderItem> Items { get; set; }
    }
}
