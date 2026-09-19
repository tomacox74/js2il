using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype.valueOf;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Boolean.prototype.valueOf") { }

    [Fact(DisplayName = "S15.6.4.3_A2_T1")]
    public Task S15_6_4_3_A2_T1()
        => ExecutionTestFromFile("S15.6.4.3_A2_T1");

    [Fact(DisplayName = "S15.6.4.3_A2_T2")]
    public Task S15_6_4_3_A2_T2()
        => ExecutionTestFromFile("S15.6.4.3_A2_T2");

    [Fact(DisplayName = "S15.6.4.3_A2_T3")]
    public Task S15_6_4_3_A2_T3()
        => ExecutionTestFromFile("S15.6.4.3_A2_T3");

    [Fact(DisplayName = "S15.6.4.3_A2_T4")]
    public Task S15_6_4_3_A2_T4()
        => ExecutionTestFromFile("S15.6.4.3_A2_T4");

    [Fact(DisplayName = "S15.6.4.3_A2_T5")]
    public Task S15_6_4_3_A2_T5()
        => ExecutionTestFromFile("S15.6.4.3_A2_T5");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
