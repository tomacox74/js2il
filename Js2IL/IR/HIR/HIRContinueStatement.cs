namespace Js2IL.HIR;

public sealed class HIRContinueStatement : HIRStatement
{
    public HIRContinueStatement(string? label = null)
    {
        Label = label;
    }

    public string? Label { get; }
}
