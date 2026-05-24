using Js2IL.Test262.Tests.built_ins;


namespace Js2IL.Test262.Tests.built_ins.Array.prototype.sort;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.sort") { }

    [Fact(DisplayName = "S15.4.4.11_A1.1_T1")]
    public Task S15_4_4_11_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.1_T1");
}
