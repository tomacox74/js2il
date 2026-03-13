namespace Js2IL.HIR;

/// <summary>
/// Represents a break statement that exits the innermost loop or labeled loop.
/// </summary>
public sealed class HIRBreakStatement : HIRStatement
{
    public HIRBreakStatement(string? label = null)
    {
        Label = label;
    }

    /// <summary>
    /// Optional label name identifying the labeled statement or loop to break from.
    /// </summary>
    public string? Label { get; }
}
