namespace Js2IL.HIR;

public sealed class HIRThrowTypeErrorExpression : HIRExpression
{
    public HIRThrowTypeErrorExpression(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
