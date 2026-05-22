namespace Js2IL.HIR;

public sealed class HIRWithStatement : HIRStatement
{
    public HIRWithStatement(HIRExpression @object, HIRStatement body)
    {
        Object = @object;
        Body = body;
    }

    public HIRExpression Object { get; }

    public HIRStatement Body { get; }
}
