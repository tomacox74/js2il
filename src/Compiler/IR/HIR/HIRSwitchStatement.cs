using System.Collections.Immutable;

namespace Js2IL.HIR;

public sealed record HIRSwitchCase(HIRExpression? Test, ImmutableArray<HIRStatement> Consequent);

/// <summary>
/// Represents a switch statement.
/// </summary>
public sealed class HIRSwitchStatement : HIRStatement
{
    public HIRSwitchStatement(HIRExpression discriminant, IEnumerable<HIRSwitchCase> cases, string? scopeName = null)
    {
        Discriminant = discriminant;
        Cases = cases.ToImmutableArray();
        ScopeName = scopeName;
    }

    public HIRExpression Discriminant { get; }
    public ImmutableArray<HIRSwitchCase> Cases { get; }
    public string? ScopeName { get; }
}
