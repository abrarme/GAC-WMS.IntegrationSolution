using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
