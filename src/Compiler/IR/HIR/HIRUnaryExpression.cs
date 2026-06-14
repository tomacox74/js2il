using Acornima;

namespace Jroc.HIR;

public sealed class HIRUnaryExpression : HIRExpression
{
    public HIRUnaryExpression(Operator op, HIRExpression argument)
    {
        Operator = op;
        Argument = argument;
    }

    public Operator Operator { get; init; }

    public HIRExpression Argument { get; init; }
}
