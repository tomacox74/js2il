namespace Js2IL.HIR;

public sealed class HIRPropertyAccessExpression : HIRExpression
{
    public HIRPropertyAccessExpression(HIRExpression obj, string propertyName)
    {
        Object = obj;
        PropertyName = propertyName;
    }

    public HIRExpression Object { get; init; }
    public string PropertyName { get; init; }
}