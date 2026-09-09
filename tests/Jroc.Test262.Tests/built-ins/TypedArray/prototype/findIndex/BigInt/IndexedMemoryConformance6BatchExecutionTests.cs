using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findIndex.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.findIndex.BigInt") { }

    [Fact(DisplayName = "predicate-call-changes-value.js")]
    public Task predicate_call_changes_value() => ExecutionTestFromFile("predicate-call-changes-value");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
