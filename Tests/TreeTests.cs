
using NUnit.Framework;

using SharpTree;

namespace Tests;

public class TreeTests
{
    [Test]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1 1.1", 1)]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1", 3)]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "2", 0)]
    public void GetChildCount_Success(string input, string path, int expectedLength)
    {
        Assert.That(Tree.FromText(input)[path].Count, Is.EqualTo(expectedLength));
    }

    [Test]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1", "1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v")]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "2", "2 2.v")]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1 1.3 1.3.1", "1.3.1 1.3.1_v")]
    public void ConvertToString_Success(string input, string path, string expectedNode)
    {
        Assert.That(Tree.FromText(input)[path].ToString(), Is.EqualTo(expectedNode));
    }

    [Test]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1\n\t\t1.3.2 1.3.2_v\n2 2.v", "1 1.3 1.3.1", "")]
    public void GetNodeWithoutValue_Success(string input, string path, string expectedValue)
    {
        Assert.That(Tree.FromText(input)[path].Value, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\n\t\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1 1.1 1.1.1", "1.1.1 1.1.1_v")]
    [TestCase("1 1_v\n\t1.1 1.1_v\n\t\t1.1.1 1.1.1_v\n\n\t\n\t1.2 1.2_v\n\t1.3 1.3_v\n\t\t1.3.1 1.3.1_v\n\t\t1.3.2 1.3.2_v\n2 2.v", "1 1.2", "1.2 1.2_v")]
    public void GetNodeAroundEmptyLine_Success(string input, string path, string expectedNode)
    {
        Assert.That(Tree.FromText(input)[path].ToString(), Is.EqualTo(expectedNode));
    }
}
