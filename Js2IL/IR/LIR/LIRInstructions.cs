using Js2IL.Services.ScopesAbi;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Acornima.Ast;

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

public record LIRConstUndefined(TempVariable Result) : LIRInstruction;

public record LIRConstNull(TempVariable Result) : LIRInstruction;

public record LIRGetIntrinsicGlobal(string Name, TempVariable Result) : LIRInstruction;

public record LIRCallIntrinsic(TempVariable IntrinsicObject, string Name, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a known CLR instance method on a known receiver type.
/// The receiver temp is expected to already be of <paramref name="ReceiverClrType"/>.
/// Arguments are JS arguments (boxed as object) and may be packed by the IL emitter
/// into an object[] depending on the target method signature.
/// </summary>
public record LIRCallInstanceMethod(
    TempVariable Receiver,
    Type ReceiverClrType,
    string MethodName,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a static method on an intrinsic object (e.g., Array.isArray, Math.abs, JSON.parse).
/// The intrinsic type is resolved via IntrinsicObjectRegistry using the IntrinsicName.
/// </summary>
/// <param name="IntrinsicName">The JavaScript intrinsic name (e.g., "Array", "Math", "JSON").</param>
/// <param name="MethodName">The method name to call (e.g., "isArray", "abs", "parse").</param>
/// <param name="Arguments">The argument temps (already boxed as object).</param>
/// <param name="Result">The temp variable to store the call result.</param>
public record LIRCallIntrinsicStatic(string IntrinsicName, string MethodName, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads the current receiver ('this') for instance callables (class methods/constructors).
/// Emitted as IL 'ldarg.0'.
/// </summary>
public record LIRLoadThis(TempVariable Result) : LIRInstruction;

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
public record LIRCallFunction(Symbol FunctionSymbol, TempVariable ScopesArray, IReadOnlyList<TempVariable> Arguments, TempVariable Result, CallableId? CallableId = null) : LIRInstruction;

/// <summary>
/// Calls a function stored in a JS value (delegate) via runtime dispatch.
/// This is used for indirect calls like `const f = makeFn(); f(...)` where the callee is not a direct function binding.
/// Emits: call JavaScriptRuntime.Closure.InvokeWithArgs(object target, object[] scopes, object[] args)
/// </summary>
public record LIRCallFunctionValue(TempVariable FunctionValue, TempVariable ScopesArray, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates a JS callable value (delegate) for an ArrowFunctionExpression and binds it to a scopes array.
/// Emits: ldnull, ldftn <method>, newobj Func&lt;...&gt;::.ctor, ldloc/ldarg scopesArray, call Closure.Bind(object, object[])
/// </summary>
public record LIRCreateBoundArrowFunction(CallableId CallableId, TempVariable ScopesArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates a JS callable value (delegate) for a FunctionExpression and binds it to a scopes array.
/// Emits: ldnull, ldftn <method>, newobj Func&lt;...&gt;::.ctor, ldloc/ldarg scopesArray, call Closure.Bind(object, object[])
/// </summary>
public record LIRCreateBoundFunctionExpression(CallableId CallableId, TempVariable ScopesArray, TempVariable Result) : LIRInstruction;

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

/// <summary>
/// Converts a value (typically an object) to a JavaScript number (double) using runtime coercion.
/// </summary>
public record LIRConvertToNumber(TempVariable Source, TempVariable Result) : LIRInstruction;

/// <summary>
/// Converts a value (typically an object) to a JavaScript boolean (truthiness) using runtime coercion.
/// </summary>
public record LIRConvertToBoolean(TempVariable Source, TempVariable Result) : LIRInstruction;

/// <summary>
/// Converts a value (typically an object) to a JavaScript string using runtime coercion.
/// </summary>
public record LIRConvertToString(TempVariable Source, TempVariable Result) : LIRInstruction;

public record LIRTypeof(TempVariable Value, TempVariable Result) : LIRInstruction;

public record LIRNegateNumber(TempVariable Value, TempVariable Result) : LIRInstruction;

public record LIRBitwiseNotNumber(TempVariable Value, TempVariable Result) : LIRInstruction;

public record LIRLogicalNot(TempVariable Value, TempVariable Result) : LIRInstruction;

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
/// Branches out of a protected region (try/catch) to a target label.
/// Emitted as IL 'leave'.
/// </summary>
public record LIRLeave(int TargetLabel) : LIRInstruction;

/// <summary>
/// Ends a finally handler. Emitted as IL 'endfinally'.
/// </summary>
public record LIREndFinally : LIRInstruction;

/// <summary>
/// Stores the current exception object (on stack at catch handler entry) into a temp.
/// </summary>
public record LIRStoreException(TempVariable Result) : LIRInstruction;

/// <summary>
/// Unwraps a caught CLR Exception into a JavaScript catch value.
/// - If exception is JsThrownValueException => Value
/// - If exception is JavaScriptRuntime.Error => the exception object
/// - Otherwise rethrows (treat as compiler/runtime bug)
/// </summary>
public record LIRUnwrapCatchException(TempVariable Exception, TempVariable Result) : LIRInstruction;

/// <summary>
/// Throws a JavaScript value. If the value is a CLR Exception, throws it directly;
/// otherwise wraps it in JsThrownValueException and throws.
/// </summary>
public record LIRThrow(TempVariable Value) : LIRInstruction;

/// <summary>
/// Throws a new JavaScriptRuntime.TypeError with the provided message.
/// Used for const reassignment attempts.
/// </summary>
public record LIRThrowNewTypeError(string Message) : LIRInstruction;

/// <summary>
/// Constructs a built-in JavaScriptRuntime Error type (Error, TypeError, etc.) with optional message.
/// </summary>
public record LIRNewBuiltInError(string ErrorTypeName, TempVariable? Message, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates a new instance of a JavaScriptRuntime intrinsic type via its constructor.
/// The intrinsic type is resolved via IntrinsicObjectRegistry using IntrinsicName.
/// Supported ctor shapes are intentionally minimal and selected by arity.
/// </summary>
public record LIRNewIntrinsicObject(string IntrinsicName, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates a new instance of a user-defined JavaScript class (compiled as a .NET type).
/// Constructor tokens are resolved via <see cref="CallableRegistry"/> using <see cref="ConstructorNode"/>.
/// </summary>
public record LIRNewUserClass(
    string ClassName,
    Node ConstructorNode,
    bool NeedsScopes,
    TempVariable? ScopesArray,
    int MinArgCount,
    int MaxArgCount,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

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