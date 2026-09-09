using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findLast.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.findLast.BigInt") { }

    [Fact(DisplayName = "predicate-call-changes-value.js")]
    public Task predicate_call_changes_value() => ExecutionTestFromFile("predicate-call-changes-value");
}
