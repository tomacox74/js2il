namespace Js2IL.HIR;

public sealed class HIRMethod : HIRNode
{
    public required HIRBlock Body { get; init; } 
}