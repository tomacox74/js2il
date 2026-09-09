using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.forEach.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.forEach.BigInt") { }

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance.js")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "values-are-not-cached.js")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
