using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Error.prototype") { }

    [Fact(DisplayName = "S15.11.3.1_A2_T1")]
    public Task S15_11_3_1_A2_T1()
        => ExecutionTestFromFile("S15.11.3.1_A2_T1");

    [Fact(DisplayName = "S15.11.3.1_A3_T1")]
    public Task S15_11_3_1_A3_T1()
        => ExecutionTestFromFile("S15.11.3.1_A3_T1");

    [Fact(DisplayName = "S15.11.3.1_A4_T1")]
    public Task S15_11_3_1_A4_T1()
        => ExecutionTestFromFile("S15.11.3.1_A4_T1");

}
