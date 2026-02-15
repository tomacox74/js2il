using System;

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
        int sourceLine = 0,
        int sourceColumn = 0)
    {
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        Template = template ?? throw new ArgumentNullException(nameof(template));
        SourceLine = sourceLine;
        SourceColumn = sourceColumn;
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
    /// Source line number for unique call site identification (0 if unavailable).
    /// </summary>
    public int SourceLine { get; }

    /// <summary>
    /// Source column number for unique call site identification (0 if unavailable).
    /// </summary>
    public int SourceColumn { get; }
}
