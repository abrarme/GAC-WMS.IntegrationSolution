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

        [Required]
        public string CustomerIdentifier { get; set; }

        public Customer Customer { get; set; }

        [Required, MaxLength(500)]
        public string ShipmentAddress { get; set; }

        public ICollection<SalesOrderItem> Items { get; set; }
    }
}
