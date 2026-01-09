namespace Js2IL.HIR;

/// <summary>
/// Represents a do/while statement: do body while (test)
/// </summary>
public sealed class HIRDoWhileStatement : HIRStatement
{
    public HIRDoWhileStatement(HIRStatement body, HIRExpression test, string? label = null)
    {
        Body = body;
        Test = test;
        Label = label;
    }

    /// <summary>
    /// The loop body statement.
    /// </summary>
    public HIRStatement Body { get; }

    /// <summary>
    /// The test/condition expression.
    /// </summary>
    public HIRExpression Test { get; }

    /// <summary>
    /// Optional label name (e.g. label: do { ... } while (...)).
    /// </summary>
    public string? Label { get; }
}
