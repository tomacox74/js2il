using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.map") { }

    [Fact(DisplayName = "callbackfn-resize.js")]
    public Task callbackfn_resize() => ExecutionTestFromFile("callbackfn-resize");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

    [Fact(DisplayName = "return-new-typedarray-conversion-operation.js")]
    public Task return_new_typedarray_conversion_operation() => ExecutionTestFromFile("return-new-typedarray-conversion-operation");
}
