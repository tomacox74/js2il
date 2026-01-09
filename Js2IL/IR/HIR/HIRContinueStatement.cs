namespace Js2IL.HIR;

/// <summary>
/// Represents a continue statement that skips to the next iteration of the innermost loop or labeled loop.
/// </summary>
public sealed class HIRContinueStatement : HIRStatement
{
    public HIRContinueStatement(string? label = null)
    {
        Label = label;
    }

    /// <summary>
    /// Optional label name identifying the target loop to continue.
    /// </summary>
    public string? Label { get; }
}
