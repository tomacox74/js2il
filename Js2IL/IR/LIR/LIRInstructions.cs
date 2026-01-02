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

public record LIRSubNumber(TempVariable Left, TempVariable Right, TempVariable Result) : LIRInstruction;

public record LIRConstUndefined(TempVariable Result) : LIRInstruction;

public record LIRConstNull(TempVariable Result) : LIRInstruction;

public record LIRGetIntrinsicGlobal(string Name, TempVariable Result) : LIRInstruction;

public record LIRNewObjectArray(int ElementCount, TempVariable Result) : LIRInstruction;

/// <summary>
/// Begins initialization of an array element (for multi-step initialization).  This is a hint.
/// </summary>
public record LIRBeginInitArrayElement(TempVariable Array, int Index) : LIRInstruction;

public record LIRStoreElementRef(TempVariable Array, int Index, TempVariable Value) : LIRInstruction;

public record LIRCallIntrinsic(TempVariable IntrinsicObject, string Name, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a user-defined function with zero parameters.
/// The Symbol contains the BindingInfo used to look up the compiled method handle.
/// </summary>
public record LIRCallFunction(Symbol FunctionSymbol, TempVariable ScopesArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates the scopes array for function invocation.
/// For global scope calls, this is an array containing just the global scope instance.
/// </summary>
public record LIRCreateScopesArray(TempVariable GlobalScope, TempVariable Result) : LIRInstruction;

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