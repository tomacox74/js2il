using Js2IL.Services.ScopesAbi;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using System.Reflection.Metadata;

namespace Js2IL.IR;

public enum ValueStorageKind
{
    Unknown,
    UnboxedValue,
    BoxedValue,
    Reference
}

public sealed record ValueStorage(ValueStorageKind Kind, Type? ClrType = null, EntityHandle TypeHandle = default);

public abstract record LIRInstruction;

public readonly record struct TempVariable(int Index);

public record LIRConstNumber(double Value, TempVariable Result) : LIRInstruction;

public record LIRConstString(string Value, TempVariable Result) : LIRInstruction;

public record LIRConstBoolean(bool Value, TempVariable Result) : LIRInstruction;

public record LIRConstUndefined(TempVariable Result) : LIRInstruction;

public record LIRConstNull(TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a user-defined JavaScript class declaration as a runtime <see cref="System.Type"/> object.
/// This enables using a class identifier in expression position (e.g., exporting it via CommonJS).
/// IL emitter: <c>ldtoken &lt;class&gt;</c>, <c>call Type.GetTypeFromHandle</c>
/// </summary>
public record LIRGetUserClassType(string RegistryClassName, TempVariable Result) : LIRInstruction;

public record LIRGetIntrinsicGlobal(string Name, TempVariable Result) : LIRInstruction;

public record LIRCallIntrinsic(TempVariable IntrinsicObject, string Name, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a global intrinsic function exposed as a public static method on <see cref="JavaScriptRuntime.GlobalThis"/>
/// (e.g., setTimeout, clearTimeout, setImmediate).
/// Arguments are JS arguments (boxed as object). The IL emitter handles ParamArray (params object[])
/// expansion to match the reflected method signature.
/// </summary>
public record LIRCallIntrinsicGlobalFunction(string FunctionName, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

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
/// Calls a static method on an intrinsic object where the return value is intentionally ignored.
/// This is used to represent statement-level calls (e.g., throw helpers) without creating an
/// artificial/unused result temp.
/// </summary>
public record LIRCallIntrinsicStaticVoid(string IntrinsicName, string MethodName, IReadOnlyList<TempVariable> Arguments) : LIRInstruction;

/// <summary>
/// Loads the current receiver ('this') for instance callables (class methods/constructors).
/// Emitted as IL 'ldarg.0'.
/// </summary>
public record LIRLoadThis(TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads the scopes array argument for callables that receive it.
/// For static functions with scopes parameter: scopes is IL arg0.
/// For instance constructors with scopes parameter: scopes is IL arg1.
/// </summary>
public record LIRLoadScopesArgument(TempVariable Result) : LIRInstruction;

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
/// Calls a member method on a receiver via runtime dispatch.
/// This is used for method calls where the receiver type is not known at compile time,
/// e.g., `x.join(',')` when `x` is boxed as object.
/// Emits: call JavaScriptRuntime.Object.CallMember(object receiver, string methodName, object[]? args)
/// </summary>
public record LIRCallMember(TempVariable Receiver, string MethodName, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a uniquely-resolved user-defined class instance method on a receiver value.
///
/// This instruction represents an early-bound direct <c>callvirt</c> to a known MethodDefinitionHandle.
/// The receiver is assumed to be of <paramref name="ReceiverTypeHandle"/> at runtime.
///
/// Arguments are JS arguments (boxed as object). Extra args are ignored; missing args are padded with null.
/// </summary>
public record LIRCallTypedMember(
    TempVariable Receiver,
    EntityHandle ReceiverTypeHandle,
    MethodDefinitionHandle MethodHandle,
    Type ReturnClrType,
    int MaxParamCount,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a uniquely-resolved user-defined class instance method on a receiver value, with a runtime-dispatch fallback.
///
/// Semantics:
/// - If receiver <c>isinst</c> <paramref name="ReceiverTypeHandle"/>, callvirt <paramref name="MethodHandle"/>.
/// - Otherwise, fall back to <see cref="JavaScriptRuntime.Object.CallMember(object, string, object[])"/>.
///
/// Arguments are JS arguments (boxed as object). Extra args are ignored; missing args are padded with null.
/// </summary>
public record LIRCallTypedMemberWithFallback(
    TempVariable Receiver,
    string MethodName,
    EntityHandle ReceiverTypeHandle,
    MethodDefinitionHandle MethodHandle,
    Type ReturnClrType,
    int MaxParamCount,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Constructs an object from a constructor value where the constructor is not statically known.
/// Emits: call JavaScriptRuntime.Object.ConstructValue(object constructor, object[]? args)
/// </summary>
public record LIRConstructValue(TempVariable ConstructorValue, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a user-defined JavaScript class instance method directly on the implicit 'this'.
/// The method handle and max parameter count are resolved during lowering via <see cref="Js2IL.Services.ClassRegistry"/>
/// and carried through to IL emission to avoid repeating lookup work.
/// Emits: ldarg.0, [args], callvirt instance object &lt;Class&gt;::&lt;Method&gt;(...)
/// </summary>
public record LIRCallUserClassInstanceMethod(
    string RegistryClassName,
    string MethodName,
    MethodDefinitionHandle MethodHandle,
    int MaxParamCount,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a declared callable directly via its MethodDefinitionHandle (resolved via CallableRegistry).
/// This is intended for cases where runtime dispatch isn't appropriate (e.g., user-defined class static method calls).
/// The argument list must match the target method signature.
/// </summary>
public record LIRCallDeclaredCallable(CallableId CallableId, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

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

/// <summary>
/// Checks whether <see cref="Value"/> is an instance of <see cref="TargetType"/>.
/// Emits IL: <c>isinst</c> and stores the result (boxed instance or null).
/// Useful for fast-path checks without a runtime helper call.
/// </summary>
public record LIRIsInstanceOf(Type TargetType, TempVariable Value, TempVariable Result) : LIRInstruction;

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
/// Constructor tokens are resolved via <see cref="CallableRegistry"/> using <see cref="ConstructorCallableId"/>.
/// </summary>
public record LIRNewUserClass(
    string ClassName,
    string RegistryClassName,
    CallableId ConstructorCallableId,
    bool NeedsScopes,
    TempVariable? ScopesArray,
    int MinArgCount,
    int MaxArgCount,
    IReadOnlyList<TempVariable> Arguments,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value into an instance field on a user-defined JS class instance.
/// Emits: ldarg.0, <load value>, stfld
/// </summary>
public record LIRStoreUserClassInstanceField(
    string RegistryClassName,
    string FieldName,
    bool IsPrivateField,
    TempVariable Value) : LIRInstruction;

/// <summary>
/// Stores a value into a static field on a user-defined JS class.
/// Emits: <load value>, stsfld
/// </summary>
public record LIRStoreUserClassStaticField(
    string RegistryClassName,
    string FieldName,
    TempVariable Value) : LIRInstruction;

/// <summary>
/// Loads an instance field from a user-defined JavaScript class instance (the implicit 'this').
/// Emits: ldarg.0, ldfld
/// </summary>
public record LIRLoadUserClassInstanceField(
    string RegistryClassName,
    string FieldName,
    bool IsPrivateField,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a static field from a user-defined JavaScript class (compiled as a .NET type).
/// The field handle is resolved via <see cref="Js2IL.Services.ClassRegistry"/> using <see cref="RegistryClassName"/>.
/// Emits: ldsfld object <Class>::<Field>
/// </summary>
public record LIRLoadUserClassStaticField(
    string RegistryClassName,
    string FieldName,
    TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a captured variable from a field on the leaf (current) scope instance.
/// The scope instance is in IL local 0, and the field handle is looked up via BindingInfo.
/// Emits: ldloc.0 (scope instance), ldfld (field handle)
/// </summary>
public record LIRLoadLeafScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a field from the leaf (current) scope instance by name.
/// Emits: ldloc.0 (scope instance), ldfld (field handle)
/// </summary>
public record LIRLoadScopeFieldByName(string ScopeName, string FieldName, TempVariable Result) : LIRInstruction;

/// <summary>
/// Stores a value to a captured variable field on the leaf (current) scope instance.
/// The scope instance is in IL local 0, and the field handle is looked up via BindingInfo.
/// Emits: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
/// </summary>
public record LIRStoreLeafScopeField(BindingInfo Binding, FieldId Field, ScopeId Scope, TempVariable Value) : LIRInstruction;

/// <summary>
/// Stores a value to a field on the leaf (current) scope instance by name.
/// Emits: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
/// </summary>
public record LIRStoreScopeFieldByName(string ScopeName, string FieldName, TempVariable Value) : LIRInstruction;

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