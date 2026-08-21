namespace Jroc.IR;

internal sealed record LIRNaturalLoopRegion(
    int HeaderInstructionIndex,
    int PreheaderInsertionIndex,
    IReadOnlySet<int> InstructionIndices);

internal sealed class LIRLoopNestingFacts(
    int[] depthByInstruction,
    IReadOnlyList<LIRNaturalLoopRegion>? naturalLoops = null)
{
    private static readonly IReadOnlyList<LIRNaturalLoopRegion> EmptyLoops =
        Array.Empty<LIRNaturalLoopRegion>();

    public int GetDepth(int instructionIndex)
        => instructionIndex >= 0
            && instructionIndex < depthByInstruction.Length
                ? depthByInstruction[instructionIndex]
                : 0;

    public IReadOnlyList<LIRNaturalLoopRegion> NaturalLoops =>
        naturalLoops ?? EmptyLoops;
}

internal static class LIRLoopNestingAnalysis
{
    public static LIRLoopNestingFacts Analyze(MethodBodyIR methodBody)
    {
        var depthByInstruction =
            new int[methodBody.Instructions.Count];
        if (methodBody.Instructions.Count == 0)
        {
            return new LIRLoopNestingFacts(depthByInstruction);
        }

        var graph = LIRControlFlowGraph.Build(methodBody);
        var blocks = graph.Blocks;
        if (!HasPotentialBackEdge(blocks))
        {
            return new LIRLoopNestingFacts(depthByInstruction);
        }

        var roots = FindRoots(methodBody, graph);
        var dominators = ComputeDominators(blocks, roots);
        var loopBlocksByHeader =
            new Dictionary<int, HashSet<int>>();

        for (var source = 0; source < blocks.Count; source++)
        {
            foreach (var header in blocks[source].Successors)
            {
                if (!dominators[source].Contains(header))
                {
                    continue;
                }

                if (!loopBlocksByHeader.TryGetValue(
                        header,
                        out var loopBlocks))
                {
                    loopBlocks = [];
                    loopBlocksByHeader.Add(header, loopBlocks);
                }

                AddNaturalLoop(
                    blocks,
                    source,
                    header,
                    loopBlocks);
            }
        }

        foreach (var loopBlocks in loopBlocksByHeader.Values)
        {
            foreach (var blockIndex in loopBlocks)
            {
                var block = blocks[blockIndex];
                for (var instructionIndex = block.Start;
                     instructionIndex < block.End;
                     instructionIndex++)
                {
                    depthByInstruction[instructionIndex]++;
                }
            }
        }

        var naturalLoops = loopBlocksByHeader
            .Select(pair => CreateNaturalLoopRegion(
                methodBody,
                blocks,
                pair.Key,
                pair.Value))
            .Where(static loop => loop != null)
            .Select(static loop => loop!)
            .OrderBy(
                static loop => loop.InstructionIndices.Count)
            .ThenBy(
                static loop => loop.HeaderInstructionIndex)
            .ToArray();

        return new LIRLoopNestingFacts(
            depthByInstruction,
            naturalLoops);
    }

    private static LIRNaturalLoopRegion? CreateNaturalLoopRegion(
        MethodBodyIR methodBody,
        IReadOnlyList<LIRBasicBlock> blocks,
        int header,
        IReadOnlySet<int> loopBlocks)
    {
        var externalPredecessors = blocks[header].Predecessors
            .Where(predecessor => !loopBlocks.Contains(predecessor))
            .ToArray();
        if (externalPredecessors.Length != 1)
        {
            return null;
        }

        var preheader = blocks[externalPredecessors[0]];
        var insertionIndex = preheader.End;
        if (insertionIndex > preheader.Start
            && IsTerminator(
                methodBody.Instructions[insertionIndex - 1]))
        {
            insertionIndex--;
        }

        var instructionIndices = new HashSet<int>();
        foreach (var blockIndex in loopBlocks)
        {
            var block = blocks[blockIndex];
            for (var index = block.Start; index < block.End; index++)
            {
                instructionIndices.Add(index);
            }
        }

        return new LIRNaturalLoopRegion(
            blocks[header].Start,
            insertionIndex,
            instructionIndices);
    }

    private static bool IsTerminator(LIRInstruction instruction)
        => instruction is
            LIRBranch
            or LIRLeave
            or LIRBranchIfTrue
            or LIRBranchIfFalse
            or LIRAsyncStateSwitch
            or LIRGeneratorStateSwitch
            or LIRAwait
            or LIRYield
            or LIRReturn
            or LIRReturnUndefinedImmediate
            or LIRTailCallFunctionReturn
            or LIRAsyncReturnPromise
            or LIRThrow
            or LIRThrowNewTypeError
            or LIREndFinally;

    private static bool HasPotentialBackEdge(
        IReadOnlyList<LIRBasicBlock> blocks)
    {
        for (var source = 0; source < blocks.Count; source++)
        {
            if (blocks[source].Successors.Any(
                    successor => successor <= source))
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<int> FindRoots(
        MethodBodyIR methodBody,
        LIRControlFlowGraph graph)
    {
        var roots = new HashSet<int> { 0 };
        foreach (var region in methodBody.ExceptionRegions)
        {
            if (graph.TryGetLabelBlock(
                    region.HandlerStartLabelId,
                    out var handlerBlock))
            {
                roots.Add(handlerBlock);
            }
        }

        return roots;
    }

    private static HashSet<int>[] ComputeDominators(
        IReadOnlyList<LIRBasicBlock> blocks,
        IReadOnlySet<int> roots)
    {
        var allBlocks = Enumerable.Range(0, blocks.Count).ToHashSet();
        var dominators = new HashSet<int>[blocks.Count];
        for (var blockIndex = 0;
             blockIndex < blocks.Count;
             blockIndex++)
        {
            dominators[blockIndex] = roots.Contains(blockIndex)
                ? [blockIndex]
                : new HashSet<int>(allBlocks);
        }

        var changed = true;
        while (changed)
        {
            changed = false;
            for (var blockIndex = 0;
                 blockIndex < blocks.Count;
                 blockIndex++)
            {
                if (roots.Contains(blockIndex))
                {
                    continue;
                }

                var predecessors = blocks[blockIndex].Predecessors;
                var updated = predecessors.Count == 0
                    ? []
                    : new HashSet<int>(
                        dominators[predecessors.First()]);
                foreach (var predecessor in predecessors.Skip(1))
                {
                    updated.IntersectWith(dominators[predecessor]);
                }
                updated.Add(blockIndex);

                if (!dominators[blockIndex].SetEquals(updated))
                {
                    dominators[blockIndex] = updated;
                    changed = true;
                }
            }
        }

        return dominators;
    }

    private static void AddNaturalLoop(
        IReadOnlyList<LIRBasicBlock> blocks,
        int source,
        int header,
        HashSet<int> loopBlocks)
    {
        loopBlocks.Add(header);
        if (!loopBlocks.Add(source) || source == header)
        {
            return;
        }

        var pending = new Stack<int>();
        pending.Push(source);
        while (pending.Count > 0)
        {
            var blockIndex = pending.Pop();
            foreach (var predecessor in
                     blocks[blockIndex].Predecessors)
            {
                if (predecessor != header
                    && loopBlocks.Add(predecessor))
                {
                    pending.Push(predecessor);
                }
            }
        }
    }
}
