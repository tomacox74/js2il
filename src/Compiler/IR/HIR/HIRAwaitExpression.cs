namespace Jroc.HIR;

public sealed class HIRAwaitExpression : HIRExpression
{
    public HIRAwaitExpression(HIRExpression argument)
    {
        Argument = argument;
    }

    public HIRExpression Argument { get; }
}
