using System.Xml.Serialization;

namespace GAC_WMS.IntegrationSolution.Models
{
    [XmlRoot("Products")]
    public class ProductList
    {
        [XmlElement("Product")]
        public List<Product> Items { get; set; }
    }
}
