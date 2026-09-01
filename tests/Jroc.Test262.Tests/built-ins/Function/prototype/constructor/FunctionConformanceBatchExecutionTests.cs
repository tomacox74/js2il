using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.constructor;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.constructor") { }

    [Fact(DisplayName = "S15.3.4.1_A1_T1.js")]
    public Task S15_3_4_1_A1_T1() => ExecutionTestFromFile("S15.3.4.1_A1_T1");

}
