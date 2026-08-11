namespace Jroc.HIR;

public sealed class HIRLoadPrivateReceiverFieldExpression : HIRExpression
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required HIRExpression Receiver { get; init; }
}

public sealed class HIRStorePrivateReceiverFieldExpression : HIRExpression
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required HIRExpression Receiver { get; init; }
    public required HIRExpression Value { get; init; }
}
