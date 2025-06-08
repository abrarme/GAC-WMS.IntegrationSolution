using System.Dynamic;
using System.Xml.Linq;

namespace GAC_WMS.IntegrationSolution.Helper
{
    public static class XmlParser
    {
        public static List<ExpandoObject> ParseXmlToDynamicList(string xmlContent, string recordElementName)
        {
            var xdoc = XDocument.Parse(xmlContent);
            var records = new List<ExpandoObject>();

            foreach (var element in xdoc.Descendants(recordElementName))
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
}
