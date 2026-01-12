using Js2IL.Services.TwoPhaseCompilation;

namespace Js2IL.HIR;

/// <summary>
/// Stores a value into a known instance field on the current receiver ('this')
/// of a user-defined class compiled as a .NET type.
/// This is used for implicit constructor initialization (e.g., field initializers, this._scopes).
/// </summary>
public sealed class HIRStoreUserClassInstanceFieldStatement : HIRStatement
{
    public required string RegistryClassName { get; init; }
    public required string FieldName { get; init; }
    public required bool IsPrivateField { get; init; }
    public required HIRExpression Value { get; init; }
    public SourceLocation? Location { get; init; }
}
