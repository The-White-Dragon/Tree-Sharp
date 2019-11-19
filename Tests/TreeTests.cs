using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SharpTree;
using Xunit;

namespace Tests
{
	public class TreeTests
	{
		[Theory]
		[InlineData("foo\nbar\n", 3)]
		[InlineData("access\n\ttime \\2035 - 28 - 07 13:08:24\n\turl \\/ favicon.png\n\tip \\8.8.8.8\n", 5)]
		[InlineData("\\foo\n\\bar\n", 1)]
		[InlineData("foo\n", 2)]
		[InlineData("foo \\data \\data\n\tbar \\data\n\t\tbaz \\data\n", 4)]
		[InlineData("foo\n\n\n", 2)]
		[InlineData("\\foo\n", 1)]
		public void CheckLength(string input, int length)
			=> Assert.Equal(length, new Tree(input, "").Count);

		[Theory]
		[InlineData("foo\nbar\n")]
		[InlineData("foo\nbar \\some data\n\turi #1:1\n")]
		public void TestSerialization(string data)
		{
			using var mem = new MemoryStream();
			var formatter = new BinaryFormatter();

			var exp = new Tree(data, "");
			formatter.Serialize(mem, exp);
			mem.Position = 0;

			var act = formatter.Deserialize(mem);
			Assert.Equal(exp.ToString(), act.ToString());
		}

		[Theory]
		[InlineData("foo \\1\nbar \\2\n", 1, "foo \\1\n")]
		[InlineData("foo \\1\nbar \\2\n", 2, "bar \\2\n")]
		[InlineData("foo \\1\nbar \\2\n", 0, "\nfoo \\1\nbar \\2\n")]
		public void TestAccessByIndex(string tree, int index, string expect) => Assert.Equal(expect, new Tree(tree, "")[index].ToString());

		[Theory]
		[InlineData("foo \\1\nbar \\2\n", "foo", "foo \\1\n")]
		[InlineData("foo \\1\nbar \\2\n", "bar", "bar \\2\n")]
		[InlineData("foo \\1\nbar \\2\n\tbarchild \\value\n", "bar", "bar \\2\n")]
		public void TestAccessByName(string tree, string name, string expect) => Assert.Equal(expect, new Tree(tree, "")[name].ToString());
	}
}
