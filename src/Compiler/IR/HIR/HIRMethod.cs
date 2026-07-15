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

    /// <summary>
    /// Indicates that this arrow inherits the derived constructor's <c>super()</c> environment.
    /// </summary>
    public bool IsLexicallyEnclosedByDerivedConstructor { get; init; }

    public required HIRBlock Body { get; init; }
}