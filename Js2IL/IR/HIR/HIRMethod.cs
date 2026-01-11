namespace Js2IL.HIR;

public sealed class HIRMethod : HIRNode
{
    /// <summary>
    /// Ordered formal parameter patterns for this callable.
    /// This is populated during AST->HIR construction so downstream stages
    /// (HIR->LIR lowering) do not need to consult the AST.
    /// </summary>
    public IReadOnlyList<HIRPattern> Parameters { get; init; } = Array.Empty<HIRPattern>();

    public required HIRBlock Body { get; init; } 
}