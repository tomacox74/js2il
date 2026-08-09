
using System.Collections.Immutable;
using Acornima.Ast;
namespace Jroc.HIR;

public sealed class HIRCallExpression : HIRExpression
{
    public HIRCallExpression(
        HIRExpression callee,
        IEnumerable<HIRExpression> arguments,
        CallExpression? sourceCall = null)
    {
        Callee = callee;
        Arguments = arguments.ToImmutableArray();
        SourceCall = sourceCall;
    }

    public HIRExpression Callee { get; init; }
    public ImmutableArray<HIRExpression> Arguments { get; init; }
    public CallExpression? SourceCall { get; init; }
}