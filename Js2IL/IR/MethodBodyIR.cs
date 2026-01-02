namespace Js2IL.IR;

public sealed class MethodBodyIR
{
    public List<string> Parameters { get; } = new();

    /// <summary>
    /// JavaScript variable declarations in this method body.
    /// These are used only for IL lowering (locals signature + slot mapping), not by LIR instructions.
    /// </summary>
    public List<string> VariableNames { get; } = new();

    /// <summary>
    /// Storage/type information for each variable slot (indexed by slot number).
    /// </summary>
    public List<ValueStorage> VariableStorages { get; } = new();

    /// <summary>
    /// SSA value slots produced/consumed by LIR instructions.
    /// These are not source-level locals; they are value IDs.
    /// </summary>
    public List<TempVariable> Temps { get; } = new();

    /// <summary>
    /// Storage/type information for each temp slot (indexed by TempVariable.Index).
    /// </summary>
    public List<ValueStorage> TempStorages { get; } = new();

    /// <summary>
    /// Optional mapping from SSA temp index to a variable local slot index.
    /// If TempVariableSlots[i] >= 0, temp i is stored/loaded via that variable slot.
    /// </summary>
    public List<int> TempVariableSlots { get; } = new();

    public List<LIRInstruction> Instructions { get; } = new();
}