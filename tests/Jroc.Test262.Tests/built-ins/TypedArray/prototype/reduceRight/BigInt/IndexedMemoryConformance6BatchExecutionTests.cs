using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduceRight.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.reduceRight.BigInt") { }

    [Fact(DisplayName = "values-are-not-cached.js")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
