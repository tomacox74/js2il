namespace Js2IL.HIR;

public sealed class HIRPrivateAccessorAssignmentExpression : HIRExpression
{
    public required string SetterMethodName { get; init; }
    public required HIRExpression Value { get; init; }
}
