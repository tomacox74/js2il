namespace Js2IL.HIR;

public sealed class HIRThrowStatement : HIRStatement
{
    public HIRThrowStatement(HIRExpression argument)
    {
        Argument = argument;
    }

    public HIRExpression Argument { get; }
}
