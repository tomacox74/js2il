namespace Js2IL.HIR;

public sealed class HIROptionalPropertyAccessExpression : HIRExpression
{
    public HIROptionalPropertyAccessExpression(HIRExpression obj, string propertyName)
    {
        Object = obj;
        PropertyName = propertyName;
    }

    public HIRExpression Object { get; init; }
    public string PropertyName { get; init; }
}
