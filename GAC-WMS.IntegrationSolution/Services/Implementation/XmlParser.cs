using GAC_WMS.IntegrationSolution.Services.Interface;
using System.Dynamic;
using System.Xml.Linq;

public class XmlParser : IXmlParser
{
    public List<dynamic> ParseXmlToDynamicList(string xmlContent, string elementName)
    {
        var xdoc = XDocument.Parse(xmlContent);
        var records = new List<dynamic>();

        foreach (var element in xdoc.Descendants(elementName))
        {
            dynamic obj = new ExpandoObject();
            var dict = (IDictionary<string, object>)obj;

            foreach (var child in element.Elements())
            {   
                dict[child.Name.LocalName] = child.Value;
            }

            records.Add(obj);
        }

        return records;
    }
}
