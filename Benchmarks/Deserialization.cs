
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using Newtonsoft.Json;

using SharpTree;

using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn, CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[KeepBenchmarkFiles(false)]
public class Deserialization
{
    public readonly int ItemCount = 1000;

    public string TreeText = "";
    public string XmlText = "";
    public string JsonText = "";

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

    //[Benchmark]
    //[BenchmarkCategory("Tree")]
    //public Tree TreeParsing()
    //{
    //    return Tree.Parse(TreeText, "users");
    //}

    //[Benchmark]
    //[BenchmarkCategory("Xml")]
    //public UserCollection Dotnet7Xml()
    //{
    //    var x = new XmlSerializer(typeof(UserCollection));

    //    return (UserCollection)x.Deserialize(XmlReader.Create(new StringReader(XmlText)))!;
    //}

    [Benchmark]
    [BenchmarkCategory("Json")]
    public UserCollection Dotnet7Json()
    {
        return System.Text.Json.JsonSerializer.Deserialize<UserCollection>(JsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    //[Benchmark]
    //[BenchmarkCategory("Json")]
    //public UserCollection NewtonsoftJson()
    //{
    //    var serializer = Newtonsoft.Json.JsonSerializer.Create();
    //    return serializer.Deserialize<UserCollection>(new JsonTextReader(new StringReader(JsonText)))!;
    //}

    //[Benchmark]
    //[BenchmarkCategory("Json")]
    //public UserCollection JsonDotNet()
    //{
    //    var s = new Json.Net.JsonParser();
    //    s.Initialize(JsonText);

    //    return (UserCollection)s.FromJson(typeof(UserCollection));
    //}
}