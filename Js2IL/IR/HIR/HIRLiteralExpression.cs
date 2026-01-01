using Js2IL.Services;

namespace Js2IL.HIR;

public sealed class HIRLiteralExpression : HIRExpression
{
    public HIRLiteralExpression(JavascriptType type, object? value)
    {
        Kind = type;
        Value = value;
    }


    public JavascriptType Kind { get; init; }
    public object? Value { get; init; }
}