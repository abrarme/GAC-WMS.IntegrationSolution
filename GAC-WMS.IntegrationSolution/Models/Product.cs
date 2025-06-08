using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class Product
    {

        public int Id { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Dimensions { get; set; }




    }
}
