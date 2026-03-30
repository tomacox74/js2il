using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using System.Reflection.Metadata;

namespace Js2IL.IR;

// Intrinsic-related LIR instructions (globals, intrinsic types, and intrinsic calls).

public record LIRGetIntrinsicGlobal(string Name, TempVariable Result) : LIRInstruction;

/// <summary>
/// Loads a global intrinsic function (e.g., setTimeout) as a first-class value.
/// This is used when a GlobalThis static method is referenced in expression position
/// (e.g., <c>window.setTimeout = setTimeout</c>).
/// </summary>
public record LIRGetIntrinsicGlobalFunction(string FunctionName, TempVariable Result) : LIRInstruction;

public record LIRCallIntrinsic(TempVariable IntrinsicObject, string Name, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a global intrinsic function exposed as a public static method on <see cref="JavaScriptRuntime.GlobalThis"/>
/// (e.g., setTimeout, clearTimeout, setImmediate).
/// Arguments are JS arguments (boxed as object). The IL emitter handles ParamArray (params object[])
/// expansion to match the reflected method signature.
/// </summary>
public record LIRCallIntrinsicGlobalFunction(string FunctionName, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;

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
/// Calls an intrinsic static method using a pre-built <c>object[]</c> argument array.
/// This is primarily used for spread arguments in calls like <c>Math.max(...xs)</c>.
/// </summary>
public record LIRCallIntrinsicStaticWithArgsArray(string IntrinsicName, string MethodName, TempVariable ArgumentsArray, TempVariable Result) : LIRInstruction;

/// <summary>
/// Calls a static method on an intrinsic object where the return value is intentionally ignored.
/// This is used to represent statement-level calls (e.g., throw helpers) without creating an
/// artificial/unused result temp.
/// </summary>
public record LIRCallIntrinsicStaticVoid(string IntrinsicName, string MethodName, IReadOnlyList<TempVariable> Arguments) : LIRInstruction;

/// <summary>
/// Statement-level intrinsic static call using a pre-built <c>object[]</c> argument array.
/// </summary>
public record LIRCallIntrinsicStaticVoidWithArgsArray(string IntrinsicName, string MethodName, TempVariable ArgumentsArray) : LIRInstruction;

/// <summary>
/// Calls an intrinsic base-class constructor from a derived class constructor (i.e., JavaScript <c>super(...)</c>
/// where the base is an intrinsic like Array).
///
/// For Array-derived classes, this is emitted as:
/// - ldarg.0; call instance void JavaScriptRuntime.Array::.ctor()
/// - ldarg.0; newarr object[argc]; callvirt instance void JavaScriptRuntime.Array::ConstructInto(object[])
/// </summary>
public record LIRCallIntrinsicBaseConstructor(
    string IntrinsicName,
    IReadOnlyList<TempVariable> Arguments) : LIRInstruction;

/// <summary>
/// Creates a new instance of a JavaScriptRuntime intrinsic type via its constructor.
/// The intrinsic type is resolved via IntrinsicObjectRegistry using IntrinsicName.
/// Supported ctor shapes are intentionally minimal and selected by arity.
/// </summary>
public record LIRNewIntrinsicObject(string IntrinsicName, IReadOnlyList<TempVariable> Arguments, TempVariable Result) : LIRInstruction;
