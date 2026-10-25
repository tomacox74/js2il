namespace Jroc.HIR;

public enum HIRResourceKind
{
    None,
    Sync,
    Async
}

public sealed class HIRDisposeResourcesStatement(HIRResourceKind kind) : HIRStatement
{
    public HIRResourceKind Kind { get; } = kind;
}
