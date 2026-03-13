using System.Collections.Immutable;

namespace Js2IL.HIR;

public sealed record HIRSwitchCase(HIRExpression? Test, ImmutableArray<HIRStatement> Consequent);

/// <summary>
/// Represents a switch statement.
/// </summary>
public sealed class HIRSwitchStatement : HIRStatement
{
    public HIRSwitchStatement(HIRExpression discriminant, IEnumerable<HIRSwitchCase> cases)
    {
        Discriminant = discriminant;
        Cases = cases.ToImmutableArray();
    }

    public HIRExpression Discriminant { get; }
    public ImmutableArray<HIRSwitchCase> Cases { get; }
}
