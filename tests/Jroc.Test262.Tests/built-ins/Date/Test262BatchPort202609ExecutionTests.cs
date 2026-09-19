using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Date") { }

    [Fact(DisplayName = "S15.9.3.2_A1_T1")]
    public Task S15_9_3_2_A1_T1()
        => ExecutionTestFromFile("S15.9.3.2_A1_T1");

    [Fact(DisplayName = "S15.9.3.2_A2_T1")]
    public Task S15_9_3_2_A2_T1()
        => ExecutionTestFromFile("S15.9.3.2_A2_T1");

    [Fact(DisplayName = "S15.9.3.2_A3_T1.1")]
    public Task S15_9_3_2_A3_T1_1()
        => ExecutionTestFromFile("S15.9.3.2_A3_T1.1");

    [Fact(DisplayName = "S15.9.3.2_A3_T1.2")]
    public Task S15_9_3_2_A3_T1_2()
        => ExecutionTestFromFile("S15.9.3.2_A3_T1.2");

}
