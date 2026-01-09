namespace Js2IL.HIR;

/// <summary>
/// Represents a while statement: while (test) body
/// </summary>
public sealed class HIRWhileStatement : HIRStatement
{
    public HIRWhileStatement(HIRExpression test, HIRStatement body, string? label = null)
    {
        Test = test;
        Body = body;
        Label = label;
    }

    /// <summary>
    /// The test/condition expression.
    /// </summary>
    public HIRExpression Test { get; }

    /// <summary>
    /// The loop body statement.
    /// </summary>
    public HIRStatement Body { get; }

    /// <summary>
    /// Optional label name (e.g. label: while (...) { ... }).
    /// </summary>
    public string? Label { get; }
}
