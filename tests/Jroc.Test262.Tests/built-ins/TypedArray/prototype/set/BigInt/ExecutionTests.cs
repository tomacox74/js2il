using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.set.BigInt;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.set.BigInt") { }

    [Fact(DisplayName = "array-arg-targetbuffer-detached-on-tointeger-offset-throws.js")]
    public Task Port400_array_arg_targetbuffer_detached_on_tointeger_offset_throws()
        => ExecutionTestFromFile("array-arg-targetbuffer-detached-on-tointeger-offset-throws");

    [Fact(DisplayName = "array-arg-targetbuffer-detached-throws.js")]
    public Task Port400_array_arg_targetbuffer_detached_throws()
        => ExecutionTestFromFile("array-arg-targetbuffer-detached-throws");

    [Fact(DisplayName = "typedarray-arg-srcbuffer-detached-during-tointeger-offset-throws.js")]
    public Task Port400_typedarray_arg_srcbuffer_detached_during_tointeger_offset_throws()
        => ExecutionTestFromFile("typedarray-arg-srcbuffer-detached-during-tointeger-offset-throws");

    [Fact(DisplayName = "typedarray-arg-targetbuffer-detached-during-tointeger-offset-throws.js")]
    public Task Port400_typedarray_arg_targetbuffer_detached_during_tointeger_offset_throws()
        => ExecutionTestFromFile("typedarray-arg-targetbuffer-detached-during-tointeger-offset-throws");
}
