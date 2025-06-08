namespace GAC_WMS.IntegrationSolution.Services.Interface
{
    public interface IXmlParser
    {
        List<dynamic> ParseXmlToDynamicList(string xmlContent, string elementName);
    }

}
