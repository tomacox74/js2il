using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.@with.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.with.BigInt") { }

    [Fact(DisplayName = "early-type-coercion-bigint.js")]
    public Task early_type_coercion_bigint() => ExecutionTestFromFile("early-type-coercion-bigint");
}
