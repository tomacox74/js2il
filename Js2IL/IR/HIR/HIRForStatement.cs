namespace Js2IL.HIR;

/// <summary>
/// Represents a for statement: for (init; test; update) body
/// </summary>
public sealed class HIRForStatement : HIRStatement
{
    public HIRForStatement(
        HIRStatement? init,
        HIRExpression? test,
        HIRExpression? update,
        HIRStatement body,
        string? label = null)
    {
        Init = init;
        Test = test;
        Update = update;
        Body = body;
        Label = label;
    }

    /// <summary>
    /// The initialization statement (can be variable declaration or expression statement).
    /// May be null if no initialization.
    /// </summary>
    public HIRStatement? Init { get; }

    /// <summary>
    /// The test/condition expression. May be null (infinite loop).
    /// </summary>
    public HIRExpression? Test { get; }

    /// <summary>
    /// The update expression (usually i++ or similar). May be null.
    /// </summary>
    public HIRExpression? Update { get; }

    /// <summary>
    /// The loop body statement.
    /// </summary>
    public HIRStatement Body { get; }

    /// <summary>
    /// Optional label name (e.g. label: for (...) { ... }).
    /// </summary>
    public string? Label { get; }
}
