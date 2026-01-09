using System;
using System.Collections.Generic;

namespace Js2IL.HIR;

public sealed class HIRTemplateLiteralExpression : HIRExpression
{
    public HIRTemplateLiteralExpression(IReadOnlyList<string> quasis, IReadOnlyList<HIRExpression> expressions)
    {
        Quasis = quasis ?? throw new ArgumentNullException(nameof(quasis));
        Expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
    }

    /// <summary>
    /// The cooked text parts of the template literal. Typically has length Expressions.Count + 1.
    /// </summary>
    public IReadOnlyList<string> Quasis { get; }

    /// <summary>
    /// The interpolated expressions between quasi segments.
    /// </summary>
    public IReadOnlyList<HIRExpression> Expressions { get; }
}
