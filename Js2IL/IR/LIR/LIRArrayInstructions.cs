namespace Js2IL.IR;

/// <summary>
/// Creates and initializes an object array with the given elements in a single operation.
/// All element temps must be computed before this instruction executes.
/// IL emitter uses dup pattern for efficient stack-based initialization:
/// newarr Object, [dup, ldc.i4 index, ldtemp, stelem.ref]*, leaving array on stack.
/// </summary>
public record LIRBuildArray(IReadOnlyList<TempVariable> Elements, TempVariable Result) : LIRInstruction;

/// <summary>
/// Creates and initializes a JavaScriptRuntime.Array with the given elements.
/// All element temps must be computed before this instruction executes.
/// IL emitter: newobj Array(capacity), [dup, ldtemp, callvirt Add]*, leaving array on stack.
/// </summary>
public record LIRNewJsArray(IReadOnlyList<TempVariable> Elements, TempVariable Result) : LIRInstruction;

/// <summary>
/// Pushes all elements from the source array to the target array (calls JavaScriptRuntime.Array.PushRange).
/// Used for spread elements in array literals.
/// </summary>
public record LIRArrayPushRange(TempVariable TargetArray, TempVariable SourceArray) : LIRInstruction;

/// <summary>
/// Adds a single element to the target array (calls List&lt;object&gt;.Add).
/// Used for individual elements in array literals when spread elements are present.
/// </summary>
public record LIRArrayAdd(TempVariable TargetArray, TempVariable Element) : LIRInstruction;

/// <summary>
/// Gets the length property of an object (calls JavaScriptRuntime.Object.GetLength).
/// </summary>
public record LIRGetLength(TempVariable Object, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets the length of a proven JavaScriptRuntime.Array.
/// Contract: Receiver is a proven Array; Result is an unboxed double.
/// </summary>
public record LIRGetJsArrayLength(TempVariable Receiver, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets the length of a proven JavaScriptRuntime.Int32Array.
/// Contract: Receiver is a proven Int32Array; Result is an unboxed double.
/// </summary>
public record LIRGetInt32ArrayLength(TempVariable Receiver, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets an item from an object by index (calls JavaScriptRuntime.Object.GetItem).
/// </summary>
public record LIRGetItem(TempVariable Object, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets an item from an object by index and converts the result to an unboxed number.
/// Calls JavaScriptRuntime.Object.GetItemAsNumber for a direct double result, avoiding boxing.
/// Fast path for Int32Array receivers; fallback to ToNumber(GetItem(...)) for all others.
/// Contract: Result is an unboxed double.
/// </summary>
public record LIRGetItemAsNumber(TempVariable Object, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Sets an item on an object by index/key (calls JavaScriptRuntime.Object.SetItem).
/// Returns the assigned value.
/// </summary>
public record LIRSetItem(TempVariable Object, TempVariable Index, TempVariable Value, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets an element from a proven JavaScriptRuntime.Array by numeric index.
/// Contract: Receiver is a proven Array; Index is an unboxed double.
/// Result is an object (or may be coerced to a number by the IL emitter when the temp expects an unboxed double).
/// </summary>
public record LIRGetJsArrayElement(TempVariable Receiver, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Sets an element on a proven JavaScriptRuntime.Array by numeric index.
/// Contract: Receiver is a proven Array; Index is an unboxed double.
/// Result (if materialized) is the assigned value.
/// </summary>
public record LIRSetJsArrayElement(TempVariable Receiver, TempVariable Index, TempVariable Value, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets an element from a proven JavaScriptRuntime.Int32Array by numeric index.
/// Contract: Receiver is a proven Int32Array; Index is an unboxed double; Result is an unboxed double.
/// </summary>
public record LIRGetInt32ArrayElement(TempVariable Receiver, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Sets an element on a proven JavaScriptRuntime.Int32Array by numeric index.
/// Contract: Receiver is a proven Int32Array; Index/Value are unboxed doubles.
/// Result (if materialized) is the assigned value as an unboxed double.
/// </summary>
public record LIRSetInt32ArrayElement(TempVariable Receiver, TempVariable Index, TempVariable Value, TempVariable Result) : LIRInstruction;

/// <summary>
/// Gets an element from a proven JavaScriptRuntime.Int32Array using an int32 index.
/// Contract: Receiver is a proven Int32Array; Index is an unboxed int32; Result is an unboxed int32.
/// </summary>
public record LIRGetInt32ArrayElementInt(TempVariable Receiver, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Sets an element on a proven JavaScriptRuntime.Int32Array using int32 index and value.
/// Contract: Receiver is a proven Int32Array; Index and Value are unboxed int32.
/// Result (if materialized) is the assigned value as an unboxed int32.
/// </summary>
public record LIRSetInt32ArrayElementInt(TempVariable Receiver, TempVariable Index, TempVariable Value, TempVariable Result) : LIRInstruction;

/// <summary>
/// Represents a property key-value pair for object literal construction.
/// </summary>
/// <param name="Key">The property key string.</param>
/// <param name="Value">The temp variable containing the property value (boxed as object).</param>
public readonly record struct ObjectProperty(string Key, TempVariable Value);

/// <summary>
/// Creates and initializes a JavaScript object (ExpandoObject) with the given properties.
/// IL emitter: newobj ExpandoObject, [dup, ldstr key, ldtemp value, callvirt IDictionary.set_Item]*.
/// </summary>
/// <param name="Properties">The list of property key-value pairs.</param>
/// <param name="Result">The temp variable to store the created object.</param>
public record LIRNewJsObject(IReadOnlyList<ObjectProperty> Properties, TempVariable Result) : LIRInstruction;
