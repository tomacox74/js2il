using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findIndex;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.findIndex") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
