using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.exp;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.exp") { }

    [Fact(DisplayName = "S15.8.2.8_A1")]
    public Task S15_8_2_8_A1()
        => ExecutionTestFromFile("S15.8.2.8_A1");

    [Fact(DisplayName = "S15.8.2.8_A2")]
    public Task S15_8_2_8_A2()
        => ExecutionTestFromFile("S15.8.2.8_A2");

    [Fact(DisplayName = "S15.8.2.8_A3")]
    public Task S15_8_2_8_A3()
        => ExecutionTestFromFile("S15.8.2.8_A3");

    [Fact(DisplayName = "S15.8.2.8_A4")]
    public Task S15_8_2_8_A4()
        => ExecutionTestFromFile("S15.8.2.8_A4");

    [Fact(DisplayName = "S15.8.2.8_A5")]
    public Task S15_8_2_8_A5()
        => ExecutionTestFromFile("S15.8.2.8_A5");

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
