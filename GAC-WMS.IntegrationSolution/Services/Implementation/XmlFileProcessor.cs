using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Services.Interface;
using System.Xml.Serialization;

namespace GAC_WMS.IntegrationSolution.Services.Implementation
{
    public class XmlFileProcessor : IFileProcessor
    {
        private readonly IWmsClient _wmsClient;
        private readonly ILogger<XmlFileProcessor> _logger;
        private readonly IXmlParser _xmlParser;

        public XmlFileProcessor(IWmsClient wmsClient, ILogger<XmlFileProcessor> logger,IXmlParser xmlParser)
        {
            _wmsClient = wmsClient;
            _logger = logger;
            _xmlParser = xmlParser;
        }

        public async Task ProcessAsync(string filePath, string endPoint)
        {
            try
            {
                string xmlElement = "";
                if (endPoint.ToLower().Contains("product"))
                {
                    xmlElement = "Product";
                }
                else if (endPoint.ToLower().Contains("customer"))
                {
                    xmlElement = "Customer";
                }

                //var serializer = new XmlSerializer(typeof(ProductList));
                //using var reader = new StreamReader(filePath);
                //var productList = (ProductList)serializer.Deserialize(reader);
                //var products = productList.Items;

                string xml = File.ReadAllText(filePath);
                var dataList = _xmlParser.ParseXmlToDynamicList(xml, xmlElement);

                await _wmsClient.PushDataAsync(dataList, endPoint);
                FileHelper.Archive(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process XML file: {FilePath}", filePath);
                FileHelper.MoveToError(filePath, ex);
            }
        }


    }

}
