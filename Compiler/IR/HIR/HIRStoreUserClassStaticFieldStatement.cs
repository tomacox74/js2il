using Js2IL.Services.TwoPhaseCompilation;

namespace Js2IL.HIR;

/// <summary>
/// Stores a value into a known static field on a user-defined class compiled as a .NET type.
/// This is used for class static field initializers emitted into the synthesized .cctor.
/// </summary>
public sealed class HIRStoreUserClassStaticFieldStatement : HIRStatement
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required bool IsPrivateField { get; init; }
    public required HIRExpression Value { get; init; }
    public SourceLocation? Location { get; init; }
}
