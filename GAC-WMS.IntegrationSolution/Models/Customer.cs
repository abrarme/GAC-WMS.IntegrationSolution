using System.ComponentModel.DataAnnotations;

namespace GAC_WMS.IntegrationSolution.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string CustomerIdentifier { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }    

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(200)]
        public string Contact { get; set; }
    }
}
