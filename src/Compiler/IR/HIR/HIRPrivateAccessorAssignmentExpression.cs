namespace Jroc.HIR;

public sealed class HIRPrivateAccessorAssignmentExpression : HIRExpression
{
    public required HIRExpression Receiver { get; init; }
    public required string RegistryClassName { get; init; }
    public required string SetterMethodName { get; init; }
    public required HIRExpression Value { get; init; }
}
