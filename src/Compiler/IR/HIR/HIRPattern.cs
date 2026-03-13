using System.Collections.Generic;
using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public abstract class HIRPattern;

public sealed class HIRIdentifierPattern : HIRPattern
{
    public HIRIdentifierPattern(Symbol symbol)
    {
        Symbol = symbol;
    }

    public Symbol Symbol { get; }
}

public sealed class HIRDefaultPattern : HIRPattern
{
    public HIRDefaultPattern(HIRPattern target, HIRExpression @default)
    {
        Target = target;
        Default = @default;
    }

    public HIRPattern Target { get; }

    public HIRExpression Default { get; }
}

public sealed class HIRRestPattern : HIRPattern
{
    public HIRRestPattern(HIRPattern target)
    {
        Target = target;
    }

    public HIRPattern Target { get; }
}

public readonly record struct HIRObjectPatternProperty(string Key, HIRPattern Value);

public sealed class HIRObjectPattern : HIRPattern
{
    public HIRObjectPattern(IReadOnlyList<HIRObjectPatternProperty> properties, HIRRestPattern? rest)
    {
        Properties = properties;
        Rest = rest;
    }

    public IReadOnlyList<HIRObjectPatternProperty> Properties { get; }

    public HIRRestPattern? Rest { get; }
}

public sealed class HIRArrayPattern : HIRPattern
{
    public HIRArrayPattern(IReadOnlyList<HIRPattern?> elements, HIRRestPattern? rest)
    {
        Elements = elements;
        Rest = rest;
    }

    public IReadOnlyList<HIRPattern?> Elements { get; }

    public HIRRestPattern? Rest { get; }
}
