using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.length;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.length") { }

    [Fact(DisplayName = "15.3.3.2-1.js")]
    public Task _15_3_3_2_1() => ExecutionTestFromFile("15.3.3.2-1");

}
