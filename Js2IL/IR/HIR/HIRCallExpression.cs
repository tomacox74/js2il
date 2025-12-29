
using System.Collections.Immutable;
namespace Js2IL.HIR;

public sealed class HIRCallExpression : HIRExpression
{
    public HIRCallExpression(HIRExpression callee, IEnumerable<HIRExpression> arguments)
    {
        Callee = callee;
        Arguments = arguments.ToImmutableArray();
    }

    public HIRExpression Callee { get; init; }
    public ImmutableArray<HIRExpression> Arguments { get; init; }
}