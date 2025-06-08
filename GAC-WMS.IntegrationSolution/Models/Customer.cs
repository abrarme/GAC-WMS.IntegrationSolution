using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string CustomerIdentifier { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(200)]
        public string Contact { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [JsonIgnore] // prevents cycles
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        [JsonIgnore] // prevents cycles
        public ICollection<SalesOrder> SalesOrders { get; set; }


    }
}
