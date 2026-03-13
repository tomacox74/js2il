using System;
using Js2IL.Services.TwoPhaseCompilation;

namespace Js2IL.HIR;

/// <summary>
/// Represents a tagged template expression: tag`template${expr}literal`.
/// The tag expression is called with (templateObject, ...substitutionValues).
/// </summary>
public sealed class HIRTaggedTemplateExpression : HIRExpression
{
    public HIRTaggedTemplateExpression(
        HIRExpression tag, 
        HIRTemplateLiteralExpression template,
        SourceLocation? location = null)
    {
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        Template = template ?? throw new ArgumentNullException(nameof(template));
        Location = location;
    }

    /// <summary>
    /// The tag function expression to be called.
    /// </summary>
    public HIRExpression Tag { get; }

    /// <summary>
    /// The template literal containing quasis (cooked strings) and interpolated expressions.
    /// </summary>
    public HIRTemplateLiteralExpression Template { get; }

    /// <summary>
    /// Source location of the tagged template expression for deterministic call-site identity.
    /// </summary>
    public SourceLocation? Location { get; }
}
