namespace Js2IL.HIR;

/// <summary>
/// Represents the JavaScript <c>super</c> keyword in expression position.
/// Currently used for:
/// - <c>super(...)</c> in derived class constructors
/// - <c>super.m(...)</c> in derived class methods
/// </summary>
public sealed class HIRSuperExpression : HIRExpression
{
}