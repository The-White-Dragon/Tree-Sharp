using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTree;
internal class Parsing
{
    private readonly int ItemCount = 1000;

    private string TreeText = "";

    public void SetupData()
    {
        var treeBuilder = new StringBuilder();

        for (var i = 0; i < ItemCount; i++)
        {
            treeBuilder.Append($"{i}\n\tname John\n\tage 30\n");
        }

        TreeText = treeBuilder.ToString();
    }

    public Tree TreeSharp()
    {
        return Tree.Parse(TreeText, "users");
    }

}
