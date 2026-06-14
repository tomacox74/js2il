namespace Jroc.HIR;

public sealed class HIRPrivateAccessorAssignmentExpression : HIRExpression
{
    public required string SetterMethodName { get; init; }
    public required HIRExpression Value { get; init; }
}
