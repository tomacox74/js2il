namespace Js2IL.HIR;

public sealed class HIRIndexAssignmentExpression : HIRExpression
{
    public HIRIndexAssignmentExpression(HIRExpression @object, HIRExpression index, Acornima.Operator op, HIRExpression value)
    {
        Object = @object;
        Index = index;
        Operator = op;
        Value = value;
    }

    public HIRExpression Object { get; }

    public HIRExpression Index { get; }

    public Acornima.Operator Operator { get; }

    public HIRExpression Value { get; }
}
