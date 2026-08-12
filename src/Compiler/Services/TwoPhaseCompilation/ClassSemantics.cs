namespace Jroc.Services.TwoPhaseCompilation;

/// <summary>
/// AST-independent class facts established during callable discovery.
/// </summary>
public sealed record ClassSemantics(
    string Name,
    string RegistryClassName,
    bool IsExpression,
    bool IsDerived,
    CallableId Constructor,
    IReadOnlyList<ClassMethodSemantics> Methods,
    string? BaseClassRegistryName = null,
    string? BaseIntrinsicName = null,
    bool RequiresParentScopes = false);

/// <summary>
/// AST-independent facts required to target a generated class method.
/// </summary>
public sealed record ClassMethodSemantics(
    string PropertyName,
    CallableId Callable,
    bool IsStatic,
    bool IsPrivate);
