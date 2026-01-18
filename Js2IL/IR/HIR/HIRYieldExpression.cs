namespace Js2IL.HIR;

public sealed class HIRYieldExpression : HIRExpression
{
    public HIRYieldExpression(HIRExpression? argument, bool isDelegate)
    {
        Argument = argument;
        IsDelegate = isDelegate;
    }

    public HIRExpression? Argument { get; }

    public bool IsDelegate { get; }
}
