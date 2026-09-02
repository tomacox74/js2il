using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.flat;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype.flat") { }

    [Fact(DisplayName = "array-like-objects.js")]
    public Task array_like_objects() => ExecutionTestFromFile("array-like-objects");

    [Fact(DisplayName = "bound-function-call.js")]
    public Task bound_function_call() => ExecutionTestFromFile("bound-function-call");

    [Fact(DisplayName = "call-with-boolean.js")]
    public Task call_with_boolean() => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "target-array-with-non-writable-property.js")]
    public Task target_array_with_non_writable_property() => ExecutionTestFromFile("target-array-with-non-writable-property");

}
