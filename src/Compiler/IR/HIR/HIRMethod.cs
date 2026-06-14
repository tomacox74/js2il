namespace Jroc.HIR;

public sealed class HIRMethod : HIRNode
{
    /// <summary>
    /// Ordered formal parameter patterns for this callable.
    /// This is populated during AST->HIR construction so downstream stages
    /// (HIR->LIR lowering) do not need to consult the AST.
    /// </summary>
    public IReadOnlyList<HIRPattern> Parameters { get; init; } = Array.Empty<HIRPattern>();

    /// <summary>
    /// HIR representation of a derived class constructor's heritage expression.
    /// This is populated by HIR construction so HIR-to-LIR lowering does not synthesize HIR nodes.
    /// </summary>
    public HIRExpression? SuperClassExpression { get; init; }

    public required HIRBlock Body { get; init; } 
}