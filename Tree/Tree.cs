using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpTree;

internal static class Utils
{
    internal const int FrameInitialSize = 2;

    internal static readonly char[] Separators = new[] { Space, Indent, NewLine };

    internal const char Indent = '\t';
    internal const char NewLine = '\n';
    internal const char Space = ' ';
}

internal sealed class TreeParser
{
    public TreeParser(string text!!)
    {
        _memory = text.AsMemory();
    }

    private readonly ReadOnlyMemory<char> _memory;

    private readonly List<List<Tree>> frames = new(Utils.FrameInitialSize);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal Tree Parse(string rootName)
    {
        var text = _memory.Span;
        var pos = 0;

        AddFrame();

        while (pos < text.Length)
        {
            var frame = text[pos..];
            var length = frame.IndexOf(Utils.NewLine);

            length = length < 0 ? frame.Length : length;

            var line = text.Slice(pos, length);
            var lineStart = pos;
            pos += length + 1;

            if (line.IsWhiteSpace() || line[0] is Utils.Space)
            {
                continue;
            }

            var content = line.TrimStart();

            var indent = line.Length - content.Length;
            var level = frames.Count - 1;

            if (indent - level is 1)
            {
                AddFrame();
            }
            else if (indent < level)
            {
                CollapseBy(level - indent);
            }

            var separator = content.IndexOf(Utils.Space);

            if (separator < 0)
            {
                separator = content.Length;
            }

            var name = _memory.Slice(lineStart + indent, separator);

            var value = _memory.Slice(lineStart + indent + separator, content.Length - name.Length).TrimStart();

            Push(new Tree(name, value));
        }

        CollapseToLevel(1);

        return new Tree(rootName.AsMemory(), ReadOnlyMemory<char>.Empty, frames[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CollapseBy(int delta)
    {
        for (var j = 0; j < delta; j++)
        {
            var endedFrame = frames[^1];
            frames.RemoveAt(frames.Count - 1);

            var item = frames[^1][^1];
            item.SetChilds(endedFrame);

            frames[^1][^1] = item;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CollapseToLevel(int level)
    {
        CollapseBy(frames.Count - level);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddFrame()
    {
        frames.Add(new(Utils.FrameInitialSize));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Push(Tree item)
    {
        frames[^1].Add(item);
    }
}

public struct Tree
{
    internal Tree(in ReadOnlyMemory<char> name, in ReadOnlyMemory<char> value, IReadOnlyList<Tree>? childs = null)
    {
        _name = name;
        _value = value;
        Childs = childs ?? Array.Empty<Tree>();
    }

    private readonly ReadOnlyMemory<char> _name;

    private readonly ReadOnlyMemory<char> _value;

    public string Value => _value.ToString();

    public string Name => _name.ToString();

    public IReadOnlyList<Tree> Childs { get; private set; }

    public static Tree Parse(string text!!)
    {
        return new TreeParser(text).Parse(string.Empty);
    }

    public static Tree Parse(string text!!, string rootName)
    {
        return new TreeParser(text).Parse(rootName);
    }

    public override string ToString()
    {
        if (_name.Span.IsWhiteSpace())
        {
            var sb = new StringBuilder();
            foreach (var child in Childs)
            {
                sb.Append(child.ToString(0));
            }

            return sb.ToString().TrimEnd();
        }

        return ToString(0).TrimEnd();
    }

    private string ToString(int level)
    {
        var sb = new StringBuilder(new string(Utils.Indent, level))
            .Append(_name)
            .Append(Utils.Space)
            .Append(_value)
            .Append(Utils.NewLine);

        foreach (var child in Childs)
        {
            sb.Append(child.ToString(level + 1));
        }

        return sb.ToString();
    }

    internal void SetChilds(List<Tree> endedFrame)
    {
        Childs = endedFrame;
    }

    public Tree this[in string path] => this[path.AsSpan()];

    private Tree this[ReadOnlySpan<char> path]
    {
        get
        {
            var separator = path.IndexOf(Utils.Space);
            if (separator < 0)
            {
                for (var i = 0; i < Childs.Count; i++)
                {
                    if (Childs[i]._name.Span.Equals(path, StringComparison.InvariantCulture))
                    {
                        return Childs[i];
                    }
                }
                throw new KeyNotFoundException();
            }

            return this[path[..separator]][path[separator..].Trim()];
        }
    }

    public int Count => Childs.Count;

    //public IEnumerator<Tree> GetEnumerator()
    //{
    //    return Childs.GetEnumerator();
    //}

    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //    return Childs.GetEnumerator();
    //}
}
