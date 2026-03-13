using System;
using System.Collections.Generic;
using System.Linq;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Computes SCC groups and a deterministic topo order of SCC stages.
/// This produces a deterministic plan artifact that can be used to order compilation.
/// </summary>
public static class CompilationPlanner
{
    public static CompilationPlan ComputePlan(CallableDependencyGraph graph)
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        // Compute SCCs via Tarjan in deterministic node/edge order.
        var index = 0;
        var indexByNode = new Dictionary<CallableId, int>();
        var lowlinkByNode = new Dictionary<CallableId, int>();
        var onStack = new HashSet<CallableId>();
        var stack = new Stack<CallableId>();

        var sccs = new List<List<CallableId>>();

        void StrongConnect(CallableId v)
        {
            indexByNode[v] = index;
            lowlinkByNode[v] = index;
            index++;
            stack.Push(v);
            onStack.Add(v);

            foreach (var w in graph.GetDependencies(v))
            {
                if (!indexByNode.TryGetValue(w, out var wIndex))
                {
                    StrongConnect(w);
                    lowlinkByNode[v] = Math.Min(lowlinkByNode[v], lowlinkByNode[w]);
                }
                else if (onStack.Contains(w))
                {
                    lowlinkByNode[v] = Math.Min(lowlinkByNode[v], wIndex);
                }
            }

            if (lowlinkByNode[v] == indexByNode[v])
            {
                var scc = new List<CallableId>();
                while (true)
                {
                    var w = stack.Pop();
                    onStack.Remove(w);
                    scc.Add(w);
                    if (Equals(w, v))
                    {
                        break;
                    }
                }

                // Stable ordering inside SCC
                scc.Sort((a, b) => string.CompareOrdinal(a.UniqueKey, b.UniqueKey));
                sccs.Add(scc);
            }
        }

        // Visit nodes in stable discovery order.
        foreach (var node in graph.NodesInStableOrder.Where(node => !indexByNode.ContainsKey(node)))
        {
            StrongConnect(node);
        }

        // Build membership map
        var membership = new Dictionary<CallableId, int>();
        for (var i = 0; i < sccs.Count; i++)
        {
            foreach (var member in sccs[i])
            {
                membership[member] = i;
            }
        }

        // Build SCC DAG edges and indegrees
        var sccEdgeSets = new Dictionary<int, SortedSet<int>>();
        var indegree = new int[sccs.Count];

        foreach (var caller in graph.NodesInStableOrder)
        {
            var callerScc = membership[caller];
            foreach (var callee in graph.GetDependencies(caller))
            {
                var calleeScc = membership[callee];
                if (callerScc == calleeScc)
                {
                    continue;
                }

                if (!sccEdgeSets.TryGetValue(callerScc, out var targets))
                {
                    targets = new SortedSet<int>();
                    sccEdgeSets[callerScc] = targets;
                }

                if (targets.Add(calleeScc))
                {
                    indegree[calleeScc]++;
                }
            }
        }

        // Deterministic Kahn topo sort over SCC ids.
        // Tie-break by each SCC's smallest member UniqueKey.
        string SccKey(int sccId) => sccs[sccId].Count == 0 ? "" : sccs[sccId][0].UniqueKey;
        var ready = new List<int>();
        for (var sccId = 0; sccId < sccs.Count; sccId++)
        {
            if (indegree[sccId] == 0)
            {
                ready.Add(sccId);
            }
        }
        ready.Sort((a, b) => string.CompareOrdinal(SccKey(a), SccKey(b)));

        var topoSccs = new List<int>(sccs.Count);
        while (ready.Count > 0)
        {
            var next = ready[0];
            ready.RemoveAt(0);
            topoSccs.Add(next);

            if (!sccEdgeSets.TryGetValue(next, out var targets))
            {
                continue;
            }

            foreach (var target in targets)
            {
                indegree[target]--;
                if (indegree[target] == 0)
                {
                    ready.Add(target);
                }
            }

            ready.Sort((a, b) => string.CompareOrdinal(SccKey(a), SccKey(b)));
        }

        // Create stages in topo SCC order.
        var stages = topoSccs
            .Select((sccId, topoIndex) => new CompilationPlanStage(sccId, sccs[sccId].AsReadOnly()))
            .ToList()
            .AsReadOnly();

        return new CompilationPlan
        {
            Graph = graph,
            Stages = stages,
            SccMembership = membership
        };
    }
}
