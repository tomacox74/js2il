using System.Collections.Immutable;

namespace Jroc.HIR;

public sealed class HIRCallExpression : HIRExpression
{
    public HIRCallExpression(
        HIRExpression callee,
        IEnumerable<HIRExpression> arguments,
        HIRStableDirectCallableTarget? stableDirectCallableTarget = null)
    {
        Callee = callee;
        Arguments = arguments.ToImmutableArray();
        StableDirectCallableTarget = stableDirectCallableTarget;
    }

    public HIRExpression Callee { get; init; }
    public ImmutableArray<HIRExpression> Arguments { get; init; }
    public HIRStableDirectCallableTarget? StableDirectCallableTarget { get; init; }
}
