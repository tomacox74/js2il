using System.Collections.Immutable;

namespace Jroc.HIR;

public sealed class HIRCallExpression : HIRExpression
{
    public HIRCallExpression(
        HIRExpression callee,
        IEnumerable<HIRExpression> arguments,
        HIRStableDirectCallableTarget? stableDirectCallableTarget = null,
        HIRStaticClassMethodTarget? staticClassMethodTarget = null)
    {
        Callee = callee;
        Arguments = arguments.ToImmutableArray();
        StableDirectCallableTarget = stableDirectCallableTarget;
        StaticClassMethodTarget = staticClassMethodTarget;
    }

    public HIRExpression Callee { get; init; }
    public ImmutableArray<HIRExpression> Arguments { get; init; }
    public HIRStableDirectCallableTarget? StableDirectCallableTarget { get; init; }
    public HIRStaticClassMethodTarget? StaticClassMethodTarget { get; init; }
}
