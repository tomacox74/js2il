namespace Js2IL.HIR;

public sealed class HIRBreakStatement : HIRStatement
{
    public HIRBreakStatement(string? label = null)
    {
        Label = label;
    }

    public string? Label { get; }
}
