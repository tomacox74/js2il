namespace Js2IL.HIR;

public sealed class HIRPrivateFieldAssignmentExpression : HIRExpression
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required HIRExpression Value { get; init; }
}
