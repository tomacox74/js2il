namespace Js2IL.IR;

public sealed class MethodBodyIR
{
    /// <summary>
    /// Whether this callable is an async function.
    /// When true, the callable is lowered to a state machine with entry method + MoveNext.
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// Async state machine metadata (only populated when IsAsync is true).
    /// Contains await points and state-related information for IL emission.
    /// </summary>
    public AsyncStateMachineInfo? AsyncInfo { get; set; }

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

    /// <summary>
    /// Exception handling regions (try/catch/finally) to be emitted into the IL EH table.
    /// Label ids refer to LIRLabel ids.
    /// </summary>
    public List<ExceptionRegionInfo> ExceptionRegions { get; } = new();

    /// <summary>
    /// Optional epilogue label id used for return statements inside try/finally.
    /// When set, IL emission will translate returns to store+leave and emit an epilogue block.
    /// </summary>
    public int? ReturnEpilogueLabelId { get; set; }

    /// <summary>
    /// Whether this method body needs a leaf scope local in slot 0.
    /// When true, the IL emitter must reserve local 0 for the scope instance.
    /// </summary>
    public bool NeedsLeafScopeLocal { get; set; }

    /// <summary>
    /// The scope ID of the leaf scope class, if NeedsLeafScopeLocal is true.
    /// The actual TypeDefinitionHandle is resolved via ScopeMetadataRegistry during IL emission.
    /// </summary>
    public ScopeId LeafScopeId { get; set; }

    /// <summary>
    /// Set of variable slots that are only assigned once (at initialization).
    /// Variables in this set are safe to inline even when backed by a slot,
    /// because their value cannot change between definition and use.
    /// This enables optimizations like inlining LIRConvertToObject for const variables.
    /// </summary>
    public HashSet<int> SingleAssignmentSlots { get; } = new();
}