using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpTree
{
	internal interface iTree : ISerializable
	{
		string Name { get; }

		string Value { get; }

		string BaseUri { get; }

		uint Row { get; }

		uint Col { get; }

		List<Tree> Childs { get; }
	}
}
