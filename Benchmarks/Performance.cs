
using BenchmarkDotNet.Attributes;

using SharpTree;

using System.Text;
using System.Text.Json;
using System.Xml;

[MemoryDiagnoser]
public class Performance
{
    public string? TreeText;
    public string? XmlText;
    public string? JsonText;

    [GlobalSetup(Targets = new[] { nameof(TreeParsing) })]
    public void SetupTreeText()
    {
        var treeBuilder = new StringBuilder();

        for (var i = 0; i < 1000; i++)
        {
            treeBuilder.Append($"{i + 1}\n\tname John\n\tage 30\n");
        }

        TreeText = treeBuilder.ToString();
    }

    [GlobalSetup(Target = nameof(XmlParsing))]
    public void SetupXmlText()
    {
        var xmlBuilder = new StringBuilder("<users>");

        for (var i = 0; i < 1000; i++)
        {
            xmlBuilder.Append("\t<user>\n\t\t<name>John</name>\n\t\t<age>30</age>\n\t</user>\n");
        }

        XmlText = xmlBuilder.Append("</users>").ToString();
    }

    [GlobalSetup(Target = nameof(JsonParsing))]
    public void SetupJsonText()
    {
        var jsonBuilder = new StringBuilder(@"{ ""users"" : [");

        for (var i = 0; i < 1000; i++)
        {
            jsonBuilder.Append("{\t\"name\" : \"John\",\n\t\"age\" : 30\n},");
        }

        JsonText = jsonBuilder.ToString().TrimEnd(',') + "]}";
    }

    [Benchmark]
    public Tree TreeParsing()
    {
        return Tree.FromText(in TreeText!, "users");
    }

    [Benchmark]
    public XmlDocument XmlParsing()
    {
        var doc = new XmlDocument();
        doc.LoadXml(XmlText!);
        return doc;
    }

    [Benchmark]
    public JsonDocument JsonParsing()
    {
        return JsonDocument.Parse(JsonText!);
    }
}