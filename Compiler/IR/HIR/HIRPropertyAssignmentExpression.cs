namespace Js2IL.HIR;

public sealed class HIRPropertyAssignmentExpression : HIRExpression
{
    public HIRPropertyAssignmentExpression(HIRExpression @object, string propertyName, Acornima.Operator op, HIRExpression value)
    {
        Object = @object;
        PropertyName = propertyName;
        Operator = op;
        Value = value;
    }

    public HIRExpression Object { get; }

    public string PropertyName { get; }

    public Acornima.Operator Operator { get; }

    public HIRExpression Value { get; }
}
