using System.Collections.Generic;

namespace Js2IL.HIR;

public sealed class HIRSequenceExpression : HIRExpression
{
    public HIRSequenceExpression(IReadOnlyList<HIRExpression> expressions)
    {
        Expressions = expressions;
    }

    public IReadOnlyList<HIRExpression> Expressions { get; init; }
}
