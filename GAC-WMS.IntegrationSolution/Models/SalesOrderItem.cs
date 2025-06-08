using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
   
    public class SalesOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int SalesOrderId { get; set; }


        [Required]
        public int ProductId { get; set; }


        [Required]
        public int Quantity { get; set; }
    }
}
