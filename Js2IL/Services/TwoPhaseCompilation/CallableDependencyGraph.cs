using System.Collections.Generic;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Immutable directed dependency graph of callables: Caller -> Callee.
/// Nodes are represented by <see cref="CallableId"/>.
/// </summary>
public sealed class CallableDependencyGraph
{
    private readonly IReadOnlyList<CallableId> _nodesInStableOrder;
    private readonly IReadOnlyDictionary<CallableId, IReadOnlyList<CallableId>> _edges;

    public CallableDependencyGraph(
        IReadOnlyList<CallableId> nodesInStableOrder,
        IReadOnlyDictionary<CallableId, IReadOnlyList<CallableId>> edges)
    {
        _nodesInStableOrder = nodesInStableOrder ?? throw new ArgumentNullException(nameof(nodesInStableOrder));
        _edges = edges ?? throw new ArgumentNullException(nameof(edges));
    }

    public IReadOnlyList<CallableId> NodesInStableOrder => _nodesInStableOrder;

    public IReadOnlyDictionary<CallableId, IReadOnlyList<CallableId>> Edges => _edges;

    public IReadOnlyList<CallableId> GetDependencies(CallableId caller)
    {
        return _edges.TryGetValue(caller, out var deps) ? deps : Array.Empty<CallableId>();
    }
}
