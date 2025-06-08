using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class Product
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; }
            
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }   

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string Dimensions { get; set; }
    }
}
