namespace Js2IL.IR;

/// <summary>
/// Information about a synchronous generator state machine, populated during HIR->LIR lowering.
/// Contains yield points and resume label mapping.
/// </summary>
public sealed class GeneratorStateMachineInfo
{
    public List<YieldPointInfo> YieldPoints { get; } = new();

    public Dictionary<int, int> ResumeLabels { get; } = new();

    private int _nextResumeStateId = 1;

    public int MaxResumeStateId => _nextResumeStateId - 1;

    public int AllocateResumeStateId() => _nextResumeStateId++;

    public void RegisterResumeLabel(int stateId, int labelId)
    {
        ResumeLabels[stateId] = labelId;
    }

    public int YieldPointCount => YieldPoints.Count;
}

public sealed class YieldPointInfo
{
    public required int ResumeStateId { get; init; }

    public required int ResumeLabelId { get; init; }

    public required TempVariable ResultTemp { get; init; }
}
