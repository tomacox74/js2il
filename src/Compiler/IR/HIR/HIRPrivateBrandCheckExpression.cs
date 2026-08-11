namespace Jroc.HIR;

public sealed class HIRPrivateBrandCheckExpression : HIRExpression
{
    public required string RegistryClassName { get; init; }
    public required HIRExpression Value { get; init; }
}
