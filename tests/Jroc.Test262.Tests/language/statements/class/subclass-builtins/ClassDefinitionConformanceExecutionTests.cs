namespace Jroc.Test262.Tests.language.statements.test_class.subclass_builtins;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass_builtins") { }

    [Fact(DisplayName = "subclass-AggregateError.js")]
    public Task subclass_AggregateError()
        => ExecutionTest("subclass-AggregateError");

    [Fact(DisplayName = "subclass-Array.js")]
    public Task subclass_Array()
        => ExecutionTest("subclass-Array");

    [Fact(DisplayName = "subclass-ArrayBuffer.js")]
    public Task subclass_ArrayBuffer()
        => ExecutionTest("subclass-ArrayBuffer");

    [Fact(DisplayName = "subclass-BigInt64Array.js")]
    public Task subclass_BigInt64Array()
        => ExecutionTest("subclass-BigInt64Array");

    [Fact(DisplayName = "subclass-BigUint64Array.js")]
    public Task subclass_BigUint64Array()
        => ExecutionTest("subclass-BigUint64Array");

    [Fact(DisplayName = "subclass-Boolean.js")]
    public Task subclass_Boolean()
        => ExecutionTest("subclass-Boolean");

    [Fact(DisplayName = "subclass-DataView.js")]
    public Task subclass_DataView()
        => ExecutionTest("subclass-DataView");

    [Fact(DisplayName = "subclass-Date.js")]
    public Task subclass_Date()
        => ExecutionTest("subclass-Date");

    [Fact(DisplayName = "subclass-Error.js")]
    public Task subclass_Error()
        => ExecutionTest("subclass-Error");

    [Fact(DisplayName = "subclass-EvalError.js")]
    public Task subclass_EvalError()
        => ExecutionTest("subclass-EvalError");

    [Fact(DisplayName = "subclass-Float32Array.js")]
    public Task subclass_Float32Array()
        => ExecutionTest("subclass-Float32Array");

    [Fact(DisplayName = "subclass-Float64Array.js")]
    public Task subclass_Float64Array()
        => ExecutionTest("subclass-Float64Array");

    [Fact(DisplayName = "subclass-Function.js")]
    public Task subclass_Function()
        => ExecutionTest("subclass-Function");

    [Fact(DisplayName = "subclass-Int16Array.js")]
    public Task subclass_Int16Array()
        => ExecutionTest("subclass-Int16Array");

    [Fact(DisplayName = "subclass-Int32Array.js")]
    public Task subclass_Int32Array()
        => ExecutionTest("subclass-Int32Array");

    [Fact(DisplayName = "subclass-Int8Array.js")]
    public Task subclass_Int8Array()
        => ExecutionTest("subclass-Int8Array");

    [Fact(DisplayName = "subclass-Map.js")]
    public Task subclass_Map()
        => ExecutionTest("subclass-Map");

    [Fact(DisplayName = "subclass-Number.js")]
    public Task subclass_Number()
        => ExecutionTest("subclass-Number");

    [Fact(DisplayName = "subclass-Object.js")]
    public Task subclass_Object()
        => ExecutionTest("subclass-Object");

    [Fact(DisplayName = "subclass-Promise.js")]
    public Task subclass_Promise()
        => ExecutionTest("subclass-Promise");

    [Fact(DisplayName = "subclass-RangeError.js")]
    public Task subclass_RangeError()
        => ExecutionTest("subclass-RangeError");

    [Fact(DisplayName = "subclass-ReferenceError.js")]
    public Task subclass_ReferenceError()
        => ExecutionTest("subclass-ReferenceError");

    [Fact(DisplayName = "subclass-RegExp.js")]
    public Task subclass_RegExp()
        => ExecutionTest("subclass-RegExp");

    [Fact(DisplayName = "subclass-Set.js")]
    public Task subclass_Set()
        => ExecutionTest("subclass-Set");

    [Fact(DisplayName = "subclass-SharedArrayBuffer.js")]
    public Task subclass_SharedArrayBuffer()
        => ExecutionTest("subclass-SharedArrayBuffer");

    [Fact(DisplayName = "subclass-String.js")]
    public Task subclass_String()
        => ExecutionTest("subclass-String");

    [Fact(DisplayName = "subclass-SyntaxError.js")]
    public Task subclass_SyntaxError()
        => ExecutionTest("subclass-SyntaxError");

    [Fact(DisplayName = "subclass-TypeError.js")]
    public Task subclass_TypeError()
        => ExecutionTest("subclass-TypeError");

    [Fact(DisplayName = "subclass-URIError.js")]
    public Task subclass_URIError()
        => ExecutionTest("subclass-URIError");

    [Fact(DisplayName = "subclass-Uint16Array.js")]
    public Task subclass_Uint16Array()
        => ExecutionTest("subclass-Uint16Array");

    [Fact(DisplayName = "subclass-Uint32Array.js")]
    public Task subclass_Uint32Array()
        => ExecutionTest("subclass-Uint32Array");

    [Fact(DisplayName = "subclass-Uint8Array.js")]
    public Task subclass_Uint8Array()
        => ExecutionTest("subclass-Uint8Array");

    [Fact(DisplayName = "subclass-Uint8ClampedArray.js")]
    public Task subclass_Uint8ClampedArray()
        => ExecutionTest("subclass-Uint8ClampedArray");

    [Fact(DisplayName = "subclass-WeakMap.js")]
    public Task subclass_WeakMap()
        => ExecutionTest("subclass-WeakMap");

    [Fact(DisplayName = "subclass-WeakRef.js")]
    public Task subclass_WeakRef()
        => ExecutionTest("subclass-WeakRef");

    [Fact(DisplayName = "subclass-WeakSet.js")]
    public Task subclass_WeakSet()
        => ExecutionTest("subclass-WeakSet");
}
