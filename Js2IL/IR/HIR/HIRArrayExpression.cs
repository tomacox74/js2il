using System.Collections.Immutable;

namespace Js2IL.HIR;

/// <summary>
/// Represents a JavaScript array literal expression, e.g., [1, 2, 3] or ['a', 'b', ...rest].
/// </summary>
public sealed class HIRArrayExpression : HIRExpression
{
    public HIRArrayExpression(IEnumerable<HIRExpression> elements)
    {
        Elements = elements.ToImmutableArray();
    }

    /// <summary>
    /// The elements of the array literal.
    /// For spread elements, see HIRSpreadElement.
    /// </summary>
    public ImmutableArray<HIRExpression> Elements { get; init; }
}
