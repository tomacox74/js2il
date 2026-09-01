using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.internals.Construct;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.internals.Construct") { }

    [Fact(DisplayName = "derived-return-val.js")]
    public Task derived_return_val() => ExecutionTestFromFile("derived-return-val");

}
