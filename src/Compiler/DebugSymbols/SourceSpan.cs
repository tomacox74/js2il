using Acornima;
using Acornima.Ast;

namespace Js2IL.DebugSymbols;

public readonly record struct SourcePosition(int Line, int Column)
{
    public override string ToString() => $"L{Line}C{Column}";

    public static SourcePosition FromAcornima(Position position)
        => new(position.Line, position.Column + 1); // Acornima columns are 0-based
}

/// <summary>
/// A source span suitable for debug sequence points.
/// Document is a logical identifier (initially module id); later it can be mapped to a file path.
/// </summary>
public readonly record struct SourceSpan(string Document, SourcePosition Start, SourcePosition End)
{
    public override string ToString() => $"{Document}:{Start}-{End}";

    public static SourceSpan FromNode(Node node, string document)
    {
        var loc = node.Location;
        if (!string.IsNullOrWhiteSpace(loc.SourceFile))
        {
            document = loc.SourceFile;
        }
        return new SourceSpan(
            document,
            SourcePosition.FromAcornima(loc.Start),
            SourcePosition.FromAcornima(loc.End));
    }
}
