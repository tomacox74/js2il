public sealed class HIRExpressionStatement : HIRStatement
{
    public HIRExpressionStatement(HIRExpression expression)
    {
        Expression = expression;
    }

    public HIRExpression Expression { get; init; }
}