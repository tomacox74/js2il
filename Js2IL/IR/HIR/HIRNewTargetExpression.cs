namespace Js2IL.HIR;

/// <summary>
/// Represents a new.target meta property expression.
/// Returns the constructor that was invoked via 'new', or undefined for normal calls.
/// Arrow functions inherit new.target from their lexical environment.
/// </summary>
public sealed class HIRNewTargetExpression : HIRExpression
{
}
