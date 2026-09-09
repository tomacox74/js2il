using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.fill;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.fill") { }

    [Fact(DisplayName = "fill-values-conversion-operations-consistent-nan.js")]
    public Task fill_values_conversion_operations_consistent_nan() => ExecutionTestFromFile("fill-values-conversion-operations-consistent-nan");

    [Fact(DisplayName = "fill-values-conversion-operations.js")]
    public Task fill_values_conversion_operations() => ExecutionTestFromFile("fill-values-conversion-operations");
}
