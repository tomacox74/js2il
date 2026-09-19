using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Boolean") { }

    [Fact(DisplayName = "S15.6.2.1_A2")]
    public Task S15_6_2_1_A2()
        => ExecutionTestFromFile("S15.6.2.1_A2");

    [Fact(DisplayName = "S15.6.2.1_A3")]
    public Task S15_6_2_1_A3()
        => ExecutionTestFromFile("S15.6.2.1_A3");

    [Fact(DisplayName = "S15.6.2.1_A4")]
    public Task S15_6_2_1_A4()
        => ExecutionTestFromFile("S15.6.2.1_A4");

    [Fact(DisplayName = "S15.6.3_A1")]
    public Task S15_6_3_A1()
        => ExecutionTestFromFile("S15.6.3_A1");

    [Fact(DisplayName = "S15.6.3_A2")]
    public Task S15_6_3_A2()
        => ExecutionTestFromFile("S15.6.3_A2");

    [Fact(DisplayName = "S15.6.3_A3")]
    public Task S15_6_3_A3()
        => ExecutionTestFromFile("S15.6.3_A3");

    [Fact(DisplayName = "S9.2_A2_T1")]
    public Task S9_2_A2_T1()
        => ExecutionTestFromFile("S9.2_A2_T1");

    [Fact(DisplayName = "S9.2_A3_T1")]
    public Task S9_2_A3_T1()
        => ExecutionTestFromFile("S9.2_A3_T1");

    [Fact(DisplayName = "S9.2_A4_T1")]
    public Task S9_2_A4_T1()
        => ExecutionTestFromFile("S9.2_A4_T1");

    [Fact(DisplayName = "S9.2_A4_T3")]
    public Task S9_2_A4_T3()
        => ExecutionTestFromFile("S9.2_A4_T3");

    [Fact(DisplayName = "S9.2_A5_T1")]
    public Task S9_2_A5_T1()
        => ExecutionTestFromFile("S9.2_A5_T1");

    [Fact(DisplayName = "S9.2_A5_T3")]
    public Task S9_2_A5_T3()
        => ExecutionTestFromFile("S9.2_A5_T3");

    [Fact(DisplayName = "S9.2_A6_T1")]
    public Task S9_2_A6_T1()
        => ExecutionTestFromFile("S9.2_A6_T1");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "symbol-coercion")]
    public Task symbol_coercion()
        => ExecutionTestFromFile("symbol-coercion");

}
