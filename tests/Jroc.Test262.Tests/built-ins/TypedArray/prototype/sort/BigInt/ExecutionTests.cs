using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.sort.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.sort.BigInt") { }

    [Fact(DisplayName = "arraylength-internal")]
    public Task arraylength_internal()
        => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "comparefn-calls")]
    public Task comparefn_calls()
        => ExecutionTestFromFile("comparefn-calls");

    [Fact(DisplayName = "sorted-values")]
    public Task sorted_values()
        => ExecutionTestFromFile("sorted-values");
}
