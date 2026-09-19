using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Infinity;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Infinity") { }

    [Fact(DisplayName = "15.1.1.2-0")]
    public Task _15_1_1_2_0()
        => ExecutionTestFromFile("15.1.1.2-0");

    [Fact(DisplayName = "S15.1.1.2_A1")]
    public Task S15_1_1_2_A1()
        => ExecutionTestFromFile("S15.1.1.2_A1");

    [Fact(DisplayName = "S15.1.1.2_A2_T2")]
    public Task S15_1_1_2_A2_T2()
        => ExecutionTestFromFile("S15.1.1.2_A2_T2");

    [Fact(DisplayName = "S15.1.1.2_A4")]
    public Task S15_1_1_2_A4()
        => ExecutionTestFromFile("S15.1.1.2_A4");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
