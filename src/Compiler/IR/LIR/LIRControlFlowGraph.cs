using Jroc.IL;

namespace Jroc.IR;

internal sealed class LIRBasicBlock(int start, int end)
{
    public int Start { get; } = start;
    public int End { get; } = end;
    public HashSet<int> Successors { get; } = [];
    public HashSet<int> Predecessors { get; } = [];
    public int? StartLabelId { get; set; }
}

internal sealed class LIRControlFlowGraph
{
    private readonly Dictionary<int, int> _blockByLabel;

    private LIRControlFlowGraph(
        IReadOnlyList<LIRBasicBlock> blocks,
        Dictionary<int, int> blockByLabel)
    {
        Blocks = blocks;
        _blockByLabel = blockByLabel;
    }

    public IReadOnlyList<LIRBasicBlock> Blocks { get; }

    public bool TryGetLabelBlock(int labelId, out int blockIndex)
        => _blockByLabel.TryGetValue(labelId, out blockIndex);

    public static LIRControlFlowGraph Build(MethodBodyIR methodBody)
    {
        var instructions = methodBody.Instructions;
        if (instructions.Count == 0)
        {
            return new LIRControlFlowGraph(
                [],
                []);
        }

        var leaders = new SortedSet<int> { 0 };
        var labelInstructionIndices = new Dictionary<int, int>();

        for (var index = 0; index < instructions.Count; index++)
        {
            if (instructions[index] is LIRLabel label)
            {
                leaders.Add(index);
                labelInstructionIndices[label.LabelId] = index;
            }

            if (IsBlockEndingInstruction(instructions[index])
                && index + 1 < instructions.Count)
            {
                leaders.Add(index + 1);
            }
        }

        var starts = leaders.ToArray();
        var blocks = new List<LIRBasicBlock>(starts.Length);
        var blockByInstruction = new int[instructions.Count];
        for (var blockIndex = 0; blockIndex < starts.Length; blockIndex++)
        {
            var start = starts[blockIndex];
            var end = blockIndex + 1 < starts.Length
                ? starts[blockIndex + 1]
                : instructions.Count;
            var block = new LIRBasicBlock(start, end);
            if (instructions[start] is LIRLabel startLabel)
            {
                block.StartLabelId = startLabel.LabelId;
            }

            blocks.Add(block);
            for (var index = start; index < end; index++)
            {
                blockByInstruction[index] = blockIndex;
            }
        }

        var blockByLabel = labelInstructionIndices.ToDictionary(
            pair => pair.Key,
            pair => blockByInstruction[pair.Value]);
        var resumeBlockByLabel = new Dictionary<int, int>();
        for (var blockIndex = 0; blockIndex + 1 < blocks.Count; blockIndex++)
        {
            switch (instructions[blocks[blockIndex].End - 1])
            {
                case LIRAwait awaitInstruction:
                    resumeBlockByLabel[awaitInstruction.ResumeLabelId] =
                        blockIndex + 1;
                    break;
                case LIRYield yieldInstruction:
                    resumeBlockByLabel[yieldInstruction.ResumeLabelId] =
                        blockIndex + 1;
                    break;
            }
        }

        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            var last = instructions[block.End - 1];

            void AddLabelSuccessor(int labelId)
            {
                if (resumeBlockByLabel.TryGetValue(
                        labelId,
                        out var resumeBlock))
                {
                    block.Successors.Add(resumeBlock);
                }
                else if (blockByLabel.TryGetValue(
                             labelId,
                             out var successor))
                {
                    block.Successors.Add(successor);
                }
            }

            void AddFallthrough()
            {
                if (blockIndex + 1 < blocks.Count)
                {
                    block.Successors.Add(blockIndex + 1);
                }
            }

            switch (last)
            {
                case LIRBranch branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    break;
                case LIRLeave leave:
                    AddLabelSuccessor(leave.TargetLabel);
                    break;
                case LIRBranchIfTrue branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    AddFallthrough();
                    break;
                case LIRBranchIfFalse branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    AddFallthrough();
                    break;
                case LIRAsyncStateSwitch stateSwitch:
                    foreach (var label in stateSwitch.StateToLabel.Values)
                    {
                        AddLabelSuccessor(label);
                    }
                    AddLabelSuccessor(stateSwitch.DefaultLabel);
                    break;
                case LIRGeneratorStateSwitch stateSwitch:
                    foreach (var label in stateSwitch.StateToLabel.Values)
                    {
                        AddLabelSuccessor(label);
                    }
                    AddLabelSuccessor(stateSwitch.DefaultLabel);
                    break;
                case LIRAwait awaitInstruction:
                    AddFallthrough();
                    if (awaitInstruction.RejectResumeStateId is int rejectState
                        && methodBody.AsyncInfo?.ResumeLabels.TryGetValue(
                            rejectState,
                            out var rejectLabel) == true)
                    {
                        AddLabelSuccessor(rejectLabel);
                    }
                    break;
                case LIRYield:
                    AddFallthrough();
                    break;
                case LIRReturn:
                case LIRReturnUndefinedImmediate:
                case LIRTailCallFunctionReturn:
                case LIRAsyncReturnPromise:
                case LIRThrow:
                case LIRThrowNewTypeError:
                case LIREndFinally:
                    break;
                default:
                    AddFallthrough();
                    break;
            }
        }

        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            foreach (var successor in blocks[blockIndex].Successors)
            {
                blocks[successor].Predecessors.Add(blockIndex);
            }
        }

        return new LIRControlFlowGraph(
            blocks,
            blockByLabel);
    }

    private static bool IsBlockEndingInstruction(
        LIRInstruction instruction)
        => instruction is LIRBranch
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
}
