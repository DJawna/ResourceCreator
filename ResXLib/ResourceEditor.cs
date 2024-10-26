using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace ResXLib;

public class ResourceEditor {

    public Dictionary<string,string> ReadResxToDictionary(Stream resourceFile) {
        var xpathDoc = new XPathDocument(resourceFile);
        var navigator = xpathDoc.CreateNavigator();
        var iterator = navigator.Select("/root/data");

        var result = new Dictionary<string,string>();

        while(iterator.MoveNext()) {
            var node = iterator.Current;
            if (node == null) {
                throw new Exception("the xml node: /root/data, was not found");
            }
            var key = node.GetAttribute("name", "");
            var value = node.Value.Trim();
            result[key] = value;

        }
        return result;
    }

    public Stream WriteResxToDictionary(Dictionary<string, string> resourceDictionary) {
        var assembly = Assembly.GetExecutingAssembly();
        var manifestResource = assembly.GetManifestResourceStream("ResXLib.templates.Resources.resx.xml");

        var newXml = new XmlDocument();

        newXml.Load(manifestResource!);

        XmlNode rootNode = newXml.GetElementsByTagName("root","")[0]!;


        foreach((string key, string value) in resourceDictionary) {
            XmlNode newDataNode = newXml.CreateNode(XmlNodeType.Element,"data","");
            
            var nameAttribute = newXml.CreateAttribute("name");
            nameAttribute.Value = key;
            var xmlSpaceAttribute = newXml.CreateAttribute("xml:space");
            xmlSpaceAttribute.Value = "preserve";

            
            _= newDataNode.Attributes!.Append(nameAttribute);
            _= newDataNode.Attributes!.Append(xmlSpaceAttribute);

            var valueNode = newXml.CreateNode(XmlNodeType.Element,"value","");
            valueNode.InnerText = value;
            newDataNode.AppendChild(valueNode);
            rootNode.AppendChild(newDataNode);
        }
        var ms = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(ms);
        newXml.WriteTo(xmlWriter);
        xmlWriter.Close();
        ms.Position = 0;
        return ms;
    }
}
