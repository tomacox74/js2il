namespace Js2IL.HIR;

/// <summary>
/// Represents the implicit scopes array available to certain callables.
/// For user functions: scopes is IL arg0.
/// For class constructors with parent scopes: scopes is IL arg1.
/// </summary>
public sealed class HIRScopesArrayExpression : HIRExpression
{
}
