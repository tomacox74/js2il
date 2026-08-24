namespace Jroc.HIR;

public sealed class HIRPrivateLogicalAssignmentExpression : HIRExpression
{
    public required Acornima.Operator Operator { get; init; }
    public required HIRExpression Receiver { get; init; }
    public required string RegistryClassName { get; init; }
    public string? FieldName { get; init; }
    public string? GetterMethodName { get; init; }
    public string? SetterMethodName { get; init; }
    public string? MethodName { get; init; }
    public bool HasAccessor { get; init; }
    public required HIRExpression Value { get; init; }
}
