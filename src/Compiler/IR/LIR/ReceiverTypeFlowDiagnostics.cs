using Microsoft.Extensions.Logging;

namespace Jroc.IR;

internal enum ReceiverTypeFlowDiagnosticKind
{
    Merge,
    Invalidation,
    Retained,
    Specialization
}

internal sealed record ReceiverTypeFlowDiagnosticEvent(
    int InstructionIndex,
    ReceiverTypeFlowDiagnosticKind Kind,
    string Message);

internal sealed class ReceiverTypeFlowDiagnosticTrace
{
    private readonly List<ReceiverTypeFlowDiagnosticEvent> _events = [];

    public IReadOnlyList<ReceiverTypeFlowDiagnosticEvent> Events
        => _events
            .OrderBy(static item => item.InstructionIndex)
            .ThenBy(static item => item.Kind)
            .ThenBy(static item => item.Message, StringComparer.Ordinal)
            .ToArray();

    public void RecordMerge(
        int instructionIndex,
        string location,
        string inputs,
        string result)
        => Record(
            instructionIndex,
            ReceiverTypeFlowDiagnosticKind.Merge,
            $"merge @{instructionIndex} {location}: {inputs} => {result}");

    public void RecordInvalidation(
        int instructionIndex,
        string instructionName,
        string reason,
        IReadOnlyList<string> invalidatedFacts)
        => Record(
            instructionIndex,
            ReceiverTypeFlowDiagnosticKind.Invalidation,
            $"invalidate @{instructionIndex} {instructionName} reason={reason}: "
            + string.Join("; ", invalidatedFacts));

    public void RecordRetained(
        int instructionIndex,
        string instructionName,
        TempVariable receiver,
        string fact)
        => Record(
            instructionIndex,
            ReceiverTypeFlowDiagnosticKind.Retained,
            $"retain @{instructionIndex} {instructionName} receiver=t{receiver.Index}: {fact}");

    public void RecordSpecialization(
        int instructionIndex,
        string memberName,
        TempVariable receiver,
        Type receiverType,
        int loopDepth,
        bool receiverIsProvenType,
        string action)
        => Record(
            instructionIndex,
            ReceiverTypeFlowDiagnosticKind.Specialization,
            $"specialize @{instructionIndex} member={memberName} receiver=t{receiver.Index} "
            + $"candidate={receiverType.FullName} loop-depth={loopDepth} "
            + $"type-proven={receiverIsProvenType.ToString().ToLowerInvariant()} action={action}");

    public void LogTo(
        Microsoft.Extensions.Logging.ILogger logger,
        string scopeName)
    {
        foreach (var item in Events)
        {
            logger.LogInformation(
                "[ReceiverFlow] scope={ScopeName} {Message}",
                scopeName,
                item.Message);
        }
    }

    private void Record(
        int instructionIndex,
        ReceiverTypeFlowDiagnosticKind kind,
        string message)
    {
        _events.Add(
            new ReceiverTypeFlowDiagnosticEvent(
                instructionIndex,
                kind,
                message));
    }
}
