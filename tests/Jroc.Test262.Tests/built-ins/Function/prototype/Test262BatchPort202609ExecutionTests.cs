using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Function.prototype") { }

    [Fact(DisplayName = "S15.3.3.1_A1")]
    public Task S15_3_3_1_A1()
        => ExecutionTestFromFile("S15.3.3.1_A1");

    [Fact(DisplayName = "S15.3.3.1_A3")]
    public Task S15_3_3_1_A3()
        => ExecutionTestFromFile("S15.3.3.1_A3");

    [Fact(DisplayName = "S15.3.5.2_A1_T2")]
    public Task S15_3_5_2_A1_T2()
        => ExecutionTestFromFile("S15.3.5.2_A1_T2");

}
