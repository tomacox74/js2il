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
/// Gets an item from an object by index (calls JavaScriptRuntime.Object.GetItem).
/// </summary>
public record LIRGetItem(TempVariable Object, TempVariable Index, TempVariable Result) : LIRInstruction;

/// <summary>
/// Sets an item on an object by index/key (calls JavaScriptRuntime.Object.SetItem).
/// Returns the assigned value.
/// </summary>
public record LIRSetItem(TempVariable Object, TempVariable Index, TempVariable Value, TempVariable Result) : LIRInstruction;

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
