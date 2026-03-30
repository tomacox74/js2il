namespace Js2IL.HIR;

/// <summary>
/// Represents a labeled statement: label: statement
/// Used primarily to support labeled breaks (including labeled blocks).
/// </summary>
public sealed class HIRLabeledStatement : HIRStatement
{
    public HIRLabeledStatement(string label, HIRStatement body)
    {
        Label = label;
        Body = body;
    }

    public string Label { get; }
    public HIRStatement Body { get; }
}
