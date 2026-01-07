using System;
using System.Collections.Generic;
using System.Linq;

namespace Js2IL.Services.TwoPhaseCompilation;

public sealed record CompilationPlanStage(int SccId, IReadOnlyList<CallableId> Members);

/// <summary>
/// Output of Milestone 2b: SCC groups + deterministic topo-ordered stages.
/// </summary>
public sealed class CompilationPlan
{
    public required CallableDependencyGraph Graph { get; init; }

    /// <summary>
    /// Topologically ordered SCC stages. Each stage is a single callable (acyclic) or a group.
    /// </summary>
    public required IReadOnlyList<CompilationPlanStage> Stages { get; init; }

    /// <summary>
    /// SCC membership mapping used to classify SCC-internal edges.
    /// </summary>
    public required IReadOnlyDictionary<CallableId, int> SccMembership { get; init; }

    public string ToDebugString()
    {
        var lines = new List<string>();
        lines.Add($"Stages: {Stages.Count}");
        foreach (var stage in Stages)
        {
            if (stage.Members.Count == 1)
            {
                lines.Add($"  SCC#{stage.SccId}: {stage.Members[0].UniqueKey}");
            }
            else
            {
                lines.Add($"  SCC#{stage.SccId}: {{ {string.Join(", ", stage.Members.Select(m => m.UniqueKey))} }}");
            }
        }
        return string.Join(Environment.NewLine, lines);
    }
}
