using System.Collections.Generic;

namespace SharpTree
{
    internal interface ITree : IReadOnlyCollection<Tree>
    {
        string Name { get; }

        string Value { get; }

        IReadOnlyCollection<Tree> Childs { get; }
    }
}