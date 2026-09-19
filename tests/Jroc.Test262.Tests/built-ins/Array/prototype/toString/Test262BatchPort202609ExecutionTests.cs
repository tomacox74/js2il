using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toString;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.toString") { }

    [Fact(DisplayName = "S15.4.4.2_A1_T1")]
    public Task S15_4_4_2_A1_T1()
        => ExecutionTestFromFile("S15.4.4.2_A1_T1");

    [Fact(DisplayName = "S15.4.4.2_A1_T2")]
    public Task S15_4_4_2_A1_T2()
        => ExecutionTestFromFile("S15.4.4.2_A1_T2");

    [Fact(DisplayName = "S15.4.4.2_A1_T3")]
    public Task S15_4_4_2_A1_T3()
        => ExecutionTestFromFile("S15.4.4.2_A1_T3");

    [Fact(DisplayName = "S15.4.4.2_A1_T4")]
    public Task S15_4_4_2_A1_T4()
        => ExecutionTestFromFile("S15.4.4.2_A1_T4");

    [Fact(DisplayName = "S15.4.4.2_A3_T1")]
    public Task S15_4_4_2_A3_T1()
        => ExecutionTestFromFile("S15.4.4.2_A3_T1");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
