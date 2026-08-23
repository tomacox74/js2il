using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Error.prototype") { }

    [Fact(DisplayName = "S15.11.3.1_A1_T1")]
    public Task S15_11_3_1_A1_T1()
        => ExecutionTestFromFile("S15.11.3.1_A1_T1");
}
