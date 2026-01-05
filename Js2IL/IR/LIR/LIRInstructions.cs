using Js2IL.Services.ScopesAbi;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public enum ValueStorageKind
{
    Unknown,
    UnboxedValue,
    BoxedValue,
    Reference
}

public sealed record ValueStorage(ValueStorageKind Kind, Type? ClrType = null);

public abstract record LIRInstruction;

public readonly record struct TempVariable(int Index);

public record LIRConstNumber(double Value, TempVariable Result) : LIRInstruction;

public record LIRConstString(string Value, TempVariable Result) : LIRInstruction;

public record LIRConstBoolean(bool Value, TempVariable Result) : LIRInstruction;

public record LIRAddNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// String concatenation using String.Concat. Used when both operands are known to be strings.
/// </summary>
public record LIRConcatStrings(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic addition using Operators.Add runtime helper. Used when operand types are unknown.
/// </summary>
public record LIRAddDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

public record LIRSubNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

public record LIRMulNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

/// <summary>
/// Dynamic multiplication using Operators.Multiply runtime helper. Used when operand types are unknown.
/// </summary>
public record LIRMulDynamic(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

public record LIRConstUndefined(TempVariable Result) : LIRInstruction;

public record LIRConstNull(TempVariable Result) : LIRInstruction;

public record LIRGetIntrinsicGlobal(string Name, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates and initializes an object array with the given elements in a single operation.
/// All element temps must be computed before this instruction executes.
/// IL emitter uses dup pattern for efficient stack-based initialization:
/// newarr Object, [dup, ldc.i4 index, ldtemp, stelem.ref]*, leaving array on stack.
/// </summary>
public record LIRBuildArray(IReadOnlyList<TempVariable> Elements, TempVariable Result) : LIRInstruction;

public record LIRCallIntrinsic(TempVariable IntrinsicObject, string Name, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a function parameter by its index (0-based, relative to JS parameters).
/// The IL argument index is computed by the emitter based on method context
/// (static methods start at arg0, instance/nested functions at arg1 due to scopes/this).
/// </summary>
public record LIRLoadParameter(int ParameterIndex, TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value back to a function parameter by its index (0-based, relative to JS parameters).
/// Used for default parameter initialization when the argument is null/undefined.
/// The IL argument index is computed by the emitter based on method context.
/// </summary>
public record LIRStoreParameter(int ParameterIndex, TempVariable Value) : LIRInstruction;

/// <summary>
/// Calls a user-defined function with parameters.
/// The Symbol contains the BindingInfo used to look up the compiled method handle.
/// </summary>
public record LIRCallFunction(Symbol FunctionSymbol, TempVariable ScopesArray, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates the scopes array for function invocation.
/// Currently, no concrete scope instances are tracked here; the created array may contain null.
/// The <paramref name="GlobalScope"/> parameter is reserved for future use when global
/// scope tracking is implemented.
/// </summary>
[Obsolete("Use LIRBuildScopesArray instead for proper scope materialization")]
public record LIRCreateScopesArray(TempVariable GlobalScope, TempVariable Result) : LIRInstruction;

/// <summary>
/// Represents a scope slot in the scopes array along with the source of its value.
/// </summary>
/// <param name="Slot">The scope slot metadata from ScopeChainLayout.</param>
/// <param name="Source">The source of the scope instance (LeafLocal, ScopesArgument, or ThisScopes).</param>
/// <param name="SourceIndex">For ScopesArgument or ThisScopes: the index in the source array. For LeafLocal: -1.</param>
public readonly record struct ScopeSlotSource(ScopeSlot Slot, ScopeInstanceSource Source, int SourceIndex = -1);

/// <summary>
/// Where a scope instance comes from in the caller context.
/// </summary>
public enum ScopeInstanceSource
{
    /// <summary>The scope instance is in the caller's leaf local (ldloc.0).</summary>
    LeafLocal,
    /// <summary>The scope instance is in the caller's scopes argument (ldarg scopesArg, ldelem.ref).</summary>
    ScopesArgument,
    /// <summary>The scope instance is in the caller's this._scopes field (ldarg.0, ldfld _scopes, ldelem.ref).</summary>
    ThisScopes
}

/// <summary>
/// Builds the scopes array (object[]) for a function call.
/// Each slot specifies where to load the scope instance from in the caller context.
/// </summary>
/// <param name="Slots">The ordered list of scope slots with their sources. Empty if the callee doesn't need scopes.</param>
/// <param name="Result">The temp variable to store the created array.</param>
public record LIRBuildScopesArray(IReadOnlyList<ScopeSlotSource> Slots, TempVariable Result) : LIRInstruction;

public record LIRReturn(TempVariable ReturnValue) : LIRInstruction;

public record LIRConvertToObject(TempVariable Source, Type SourceType, TempVariable Result) : LIRInstruction;

public record LIRTypeof(TempVariable Value, TempVariable Result) : LIRInstruction;

public record LIRNegateNumber(TempVariable Value, TempVariable Result) : LIRInstruction;

public record LIRBitwiseNotNumber(TempVariable Value, TempVariable Result) : LIRInstruction;

// Comparison operators for numeric values (result is bool)
public record LIRCompareNumberLessThan(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberGreaterThan(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberLessThanOrEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberGreaterThanOrEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareNumberNotEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// Comparison operators for boolean values (result is bool)
public record LIRCompareBooleanEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;
public record LIRCompareBooleanNotEqual(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

// Control flow instructions
/// <summary>
/// Marks a branch target location. LabelId is assigned during lowering.
/// </summary>
public record LIRLabel(int LabelId) : LIRInstruction;

/// <summary>
/// Unconditional branch to a label.
/// </summary>
public record LIRBranch(int TargetLabel) : LIRInstruction;

/// <summary>
/// Branch to TargetLabel if Condition is false (0), otherwise fall through.
/// This matches IL's brfalse semantics.
/// </summary>
public record LIRBranchIfFalse(TempVariable Condition, int TargetLabel) : LIRInstruction;

/// <summary>
/// Branch to TargetLabel if Condition is true (non-zero), otherwise fall through.
/// This matches IL's brtrue semantics.
/// </summary>
public record LIRBranchIfTrue(TempVariable Condition, int TargetLabel) : LIRInstruction;

/// <summary>
/// Loads a captured variable from a field on the leaf (current) scope instance.
/// The scope instance is in IL local 0, and the field handle is looked up via BindingInfo.
/// Emits: ldloc.0 (scope instance), ldfld (field handle)
/// </summary>
public record LIRLoadLeafScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value to a captured variable field on the leaf (current) scope instance.
/// The scope instance is in IL local 0, and the field handle is looked up via BindingInfo.
/// Emits: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
/// </summary>
public record LIRStoreLeafScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, TempVariable Value) : LIRInstruction;

/// <summary>
/// Loads a captured variable from a field on a parent scope instance.
/// The parent scope is accessed via the scopes array parameter, indexed by the parent scope index.
/// Emits: ldarg scopes, ldc.i4 index, ldelem.ref, castclass (scope type), ldfld (field handle)
/// </summary>
public record LIRLoadParentScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, int ParentScopeIndex, TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value to a captured variable field on a parent scope instance.
/// The parent scope is accessed via the scopes array parameter, indexed by the parent scope index.
/// Emits: ldarg scopes, ldc.i4 index, ldelem.ref, castclass (scope type), ldarg/ldloc Value, stfld (field handle)
/// </summary>
public record LIRStoreParentScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, int ParentScopeIndex, TempVariable Value) : LIRInstruction;

/// <summary>
/// Creates a new instance of the leaf scope class and stores it in IL local 0.
/// This is required before any LIRLoadLeafScopeField or LIRStoreLeafScopeField instructions.
/// Emits: newobj instance void ScopeType::.ctor(), stloc.0
/// </summary>
public record LIRCreateLeafScopeInstance(ScopeId Scope) : LIRInstruction;