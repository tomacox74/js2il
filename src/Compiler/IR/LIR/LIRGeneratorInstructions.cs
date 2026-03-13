namespace Js2IL.IR;

/// <summary>
/// Represents a yield expression inside a synchronous generator.
///
/// Lowering emits:
/// - Set generator state to ResumeStateId
/// - Return iterator result { value: yielded, done: false }
/// - Resume label to continue execution on subsequent next/throw/return
/// - Load the resume value (or throw/return) as the yield-expression result
/// </summary>
public record LIRYield(
    TempVariable YieldedValue,
    int ResumeStateId,
    int ResumeLabelId,
    TempVariable Result,
    bool HandleThrowReturn = true) : LIRInstruction;

/// <summary>
/// Multi-way branch based on generator state.
/// State 0 falls through (initial entry).
/// Other states branch to resume labels.
/// </summary>
public record LIRGeneratorStateSwitch(
    IReadOnlyDictionary<int, int> StateToLabel,
    int DefaultLabel) : LIRInstruction;
