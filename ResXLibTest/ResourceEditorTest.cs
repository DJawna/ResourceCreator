using System.Reflection;
using System.Xml.XPath;
using ResXLib;

namespace LibTest;

public class ResourceEditorTest
{
    [Fact]
    public void ConvertDictToResxTest()
    {
        var testDict = new Dictionary<string,string>();

        testDict["hello"] = "world";
        testDict["alice"] = "bob";

        var subjectUnderTest = new ResourceEditor();
        using var result = subjectUnderTest.WriteResxToDictionary(testDict);


        var pathReader = new XPathDocument(result);
        var navigator = pathReader.CreateNavigator();
        var nodes = navigator.Select("/root/data");

        var reloadedDict = new Dictionary<string,string>();
        while(nodes.MoveNext()) {
            var key = nodes.Current.GetAttribute("name", "");
            var value = nodes.Current.Value;
            reloadedDict[key] = value;
        }
        Assert.Equal(testDict, reloadedDict);
    }

    [Fact]
    public void ConvertResXToDict() {
        var assembly = Assembly.GetExecutingAssembly();
        var manifestResource = assembly.GetManifestResourceStream("ResXLibTest.TestFiles.testresource.resx.xml");
        var subjectUnderTest = new ResourceEditor();
        var actualDict = subjectUnderTest.ReadResxToDictionary(manifestResource);

        var expectedDict = new Dictionary<string,string>();

        expectedDict["hello"] = "world";
        expectedDict["alice"] = "bob";

        Assert.Equal(expectedDict, actualDict);
    }
}