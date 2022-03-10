using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SharpTree;

public sealed class Tree : IReadOnlyCollection<Tree>
{
    private static readonly char[] Separators = new[] { Space, Indent, NewLine };

    private const char Indent = '\t';
    private const char NewLine = '\n';
    private const char Space = ' ';

    private const int FrameInitialSize = 2;

    private Tree(in string name, in string? value = null, IReadOnlyList<Tree>? childs = null)
    {
        Name = name;
        Value = value;
        Childs = childs ?? _emptyTreeArray;
    }

    private readonly static Tree[] _emptyTreeArray = Array.Empty<Tree>();

    public string Name { get; }

    public string? Value { get; }

    public IReadOnlyList<Tree> Childs { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static Tree FromText(in string text, in string rootName = "")
    {
        var frames = new Stack<List<Tree>>();
        frames.Push(new List<Tree>(FrameInitialSize));

        var pos = 0;

        while (pos < text.Length)
        {
            SkipWhile(text, ref pos, NewLine);
            var lineEnd = text.IndexOf(NewLine, pos);
            if (lineEnd < 0)
            {
                lineEnd = text.Length;
            }

            var tree = ParseLine(in text, in frames, ref pos, in lineEnd);

            frames.Peek().Add(tree);
        }

        return new Tree(in rootName, string.Empty, frames.Pop());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Tree ParseLine(in string text, in Stack<List<Tree>> frames, ref int pos, in int lineEnd)
    {
        var indentStart = pos;

        SkipWhile(in text, ref pos, in lineEnd, Indent);

        var indentSize = pos - indentStart;

        var level = frames.Count - 1;

        if (indentSize - level is 1)
        {
            frames.Push(new List<Tree>(FrameInitialSize));
        }
        else if (indentSize < level)
        {
            var delta = level - indentSize;
            for (var j = 0; j < delta; j++)
            {
                var endedFrame = frames.Pop();
                frames.Peek()[^1].Childs = endedFrame;
            }
        }

        var nameStartPos = pos;
        pos = text.IndexOfAny(Separators, pos);
        var name = text[nameStartPos..pos];

        SkipWhile(in text, ref pos, in lineEnd, Space);

        var value = text[pos..lineEnd];
        pos = lineEnd + 1;

        return new Tree(name, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SkipWhile(in string text, ref int pos, in int end, in char item)
    {
        while (pos < end && text[pos] == item)
        {
            pos++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SkipWhile(in string text, ref int pos, in char item)
    {
        while (pos < text.Length && text[pos] == item)
        {
            pos++;
        }
    }

    public override string ToString()
    {
        return ToString(0);
    }

    private string ToString(int level)
    {
        return $"{new string(Indent, level)}{Name}{Space}{Value}{Childs?.Aggregate("", (str, item) => $"{str}\n{item.ToString(level + 1)}") ?? ""}";
    }

    public Tree this[string path] => this[path.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    public Tree this[string[] path] => path.Length is 1 ? Childs.First(x => x.Name == path[0]) : Childs.First(x => x.Name == path[0])[path[1..]];

    public int Count => Childs.Count;

    public IEnumerator<Tree> GetEnumerator()
    {
        return Childs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Childs.GetEnumerator();
    }
}
