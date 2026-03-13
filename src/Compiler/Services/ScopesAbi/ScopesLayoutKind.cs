namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Identifies the scopes array layout convention used by a compiled callable.
/// Used during migration to allow legacy and IR pipelines to interoperate.
/// </summary>
public enum ScopesLayoutKind
{
    /// <summary>
    /// Legacy layout: whatever ordering and indexing a legacy-emitted callee expects.
    /// For top-level functions: scopes[0] = global scope instance.
    /// For nested functions: scopes[0] = global, scopes[1] = immediate parent.
    /// </summary>
    LegacyScopesLayout,

    /// <summary>
    /// Generalized layout: outermost → innermost ancestor chain.
    /// scopes[0] = global/module scope, scopes[k] = nearest lexical ancestor.
    /// Supports full ancestor chain depth for proper closure semantics.
    /// </summary>
    GeneralizedScopesLayout
}
