using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace SharpTree
{
	[Serializable]
	public class Tree : iTree
	{
		public string Name { get; private set; }

		public string Value { get; private set; }

		public string BaseUri { get; private set; }

		public uint Row { get; private set; }

		public uint Col { get; private set; }

		public List<Tree> Childs { get; private set; }

		private Tree[] _childs;

		#region Ctors

		[DebuggerStepThrough]
		protected Tree(string name, string value, List<Tree> childs, string baseUri = "", uint? row = 0, uint? col = 0)
		{
			Name = name ?? "";
			Value = value ?? "";
			Childs = childs ?? new List<Tree>();
			BaseUri = baseUri ?? "";
			Row = row ?? 0;
			Col = col ?? 0;
		}

		[DebuggerStepThrough]
		public Tree(string input, string baseUri, uint row = 1, uint col = 1)
			: this("", null, new List<Tree>(), baseUri, row, col)
		{
			var stack = new List<Tree> { this };
			var parent = this;
			while (input.Length > 0)
			{
				var name = CutUntil(ref input, "\t\n \\");

				if (name.Length > 0)
				{
					var next = new Tree(name, null, new List<Tree>(), baseUri, row, col);
					parent.Childs.Add(next);
					parent = next;
					col += (uint)(name.Length + input.Length - (input = input.TrimStart(' ')).Length);
					continue;
				}

				if (input.Length == 0)
					break;

				if (input[0] == '\\')
				{
					var val = CutUntil(ref input, "\n")[1..^0];

					if (string.IsNullOrEmpty(parent.Value))
						parent.Value = val;
					else
						parent.Value += "\n" + val;
				}

				if (input.Length == 0)
					break;

				if (input[0] != '\n')
					throw new Exception($"Unexpected symbol '{input[0]}'");

				input = input[1..^0];
				row++;

				var indent = input.Length - (input = input.TrimStart('\t')).Length;
				col = (uint)indent;

				if (indent > stack.Count)
					throw new Exception($"Too many TABs at {row}:{col}");

				stack.Add(parent);
				parent = stack[indent];
			}
		}

		#endregion

		#region Convert

		public override string ToString()
			=> pipe(new StringBuilder(), "").ToString();

		private StringBuilder pipe(StringBuilder output, string prefix = "")
		{
			if (!string.IsNullOrEmpty(Name))
				output.Append($"{Name} ");

			var chunks = Value.Length > 0 ? Value.Split('\n') : Array.Empty<string>();

			if (chunks.Length + Childs.Count == 1)
			{
				if (chunks.Length > 0)
					output.Append($"\\{chunks[0]}\n");
				else
					Childs[0].pipe(output, prefix);
			}
			else
			{
				output.Append('\n');
				if (Name.Length > 0)
					prefix += '\t';

				foreach (var chunk in chunks)
					output.Append($"{prefix}\\{chunk}\n");

				foreach (var child in Childs)
				{
					output.Append(prefix);
					child.pipe(output, prefix);
				}
			}

			return output;
		}

		#endregion

		#region Useful

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Childs = new List<Tree>(_childs ?? Array.Empty<Tree>());
			_childs = null;
		}

		private Tree Make(string name = null, string value = null, List<Tree> childs = null, string baseUri = null, uint? row = 0, uint? col = 0)
			=> new Tree(name ?? Name, value ?? Value, childs ?? Childs, baseUri ?? BaseUri, row ?? Row, col ?? Col);

		public Tree Expand() => Make(childs: new List<Tree> { new Tree("@", Uri, _expand()) });

		public string Uri => $"{BaseUri}#{Row}:{Col}";

		#endregion

		#region Interact

		public Tree this[string path] => this[path.Split(' ')];

		public Tree this[string[] path]
		{
			get
			{
				var next = new List<Tree> { this };
				foreach (var name in path)
				{
					if (next.Count == 0)
						break;

					var prev = next;
					next = new List<Tree>();
					foreach (var item in prev)
					{
						foreach (var child in item.Childs)
						{
							if (child.Name != name)
								continue;
							next.Add(child);
						}
					}
				}
				return new Tree("", "", next);
			}
		}

		public List<Tree> this[Range range] => Expand().Childs[0].Childs.GetRange(range.Start.Value, range.End.Value - range.Start.Value);

		public Tree this[int index] => Expand().Childs[0].Childs[index];

		public int Count => count();

		[DebuggerStepThrough]
		private int count()
		{
			var c = 1;
			Childs.ForEach((x) => c += x.count());
			return c;
		}

		private List<Tree> _expand()
		{
			var l = new List<Tree> { this };
			foreach (var c in Childs)
			{
				if (c.Count > 0)
					l.AddRange(c._expand());
			}
			return l;
		}

		#endregion

		#region Helper

		[DebuggerStepThrough]
		private string CutUntil(ref string str, string until)
		{
			var output = str[0..str.IndexOfAny(until.ToCharArray())];
			str = str[output.Length..^0];
			return output;
		}

		#endregion

		#region Serialization

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Value", Value);
			info.AddValue("BaseUri", BaseUri);
			info.AddValue("Row", Row);
			info.AddValue("Col", Col);
			info.AddValue("Childs", Childs.ToArray());
		}

		protected Tree(SerializationInfo info, StreamingContext streamingContext) : this("", "")
		{
			Name = (string)info.GetValue("Name", typeof(string));
			Value = (string)info.GetValue("Value", typeof(string));
			BaseUri = (string)info.GetValue("BaseUri", typeof(string));

			Row = (uint)info.GetValue("Row", typeof(uint));
			Col = (uint)info.GetValue("Col", typeof(uint));
			_childs = (Tree[])info.GetValue("Childs", typeof(Tree[]));
		}

		#endregion
	}
}
