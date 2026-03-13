namespace Js2IL.HIR;

public sealed class HIRLoadUserClassInstanceFieldExpression : HIRExpression
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required bool IsPrivateField { get; init; }
}
