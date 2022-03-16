
using System.Text;
using System.Text.Json;
using System.Xml;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using Newtonsoft.Json.Linq;

using SharpTree;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn, CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[KeepBenchmarkFiles(false)]
public class Parsing
{
    private readonly int ItemCount = 1000;

    private string TreeText = "";
    private string XmlText = "";
    private string JsonText = "";

    [GlobalSetup]
    public void SetupData()
    {
        var xmlBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"IBM437\"?>\n<UserCollection xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n");
        var jsonBuilder = new StringBuilder(@"{ ""users"" : [");
        var treeBuilder = new StringBuilder();

        for (var i = 0; i < ItemCount; i++)
        {
            xmlBuilder.Append("\t<User>\n\t\t<Name>John</Name>\n\t\t<Age>30</Age>\n\t</User>\n");
            jsonBuilder.Append("{\t\"name\" : \"John\",\n\t\"age\" : 30\n},");
            treeBuilder.Append($"{i}\n\tname John\n\tage 30\n");
        }

        XmlText = xmlBuilder.Append("</UserCollection>").ToString();
        JsonText = jsonBuilder.ToString().TrimEnd(',') + "]}";
        TreeText = treeBuilder.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("Tree")]
    public Tree TreeSharp()
    {
        return Tree.Parse(TreeText, "users");
    }

    //[Benchmark]
    //[BenchmarkCategory("Xml")]
    //public XmlDocument Dotnet7Xml()
    //{
    //    var doc = new XmlDocument();
    //    doc.LoadXml(XmlText);
    //    return doc;
    //}

    [Benchmark]
    [BenchmarkCategory("Json")]
    public JsonDocument Dotnet7Json()
    {
        return JsonDocument.Parse(JsonText);
    }

    //[Benchmark]
    //[BenchmarkCategory("Json")]
    //public JObject NewtonsoftJson()
    //{
    //    return JObject.Parse(JsonText);
    //}
}