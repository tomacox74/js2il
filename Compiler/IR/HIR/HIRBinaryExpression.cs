using Acornima;

namespace Js2IL.HIR;
public sealed class HIRBinaryExpression : HIRExpression
{
    public HIRBinaryExpression(Operator op, HIRExpression left, HIRExpression right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }

    public HIRExpression Left { get; init; }
    public HIRExpression Right { get; init; }
    public Operator Operator { get; init; }
}