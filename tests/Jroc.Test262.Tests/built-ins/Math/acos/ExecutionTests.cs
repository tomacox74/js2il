using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.acos;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.acos") { }

    [Fact(DisplayName = "S15.8.2.2_A1")]
    public Task S15_8_2_2_A1()
        => ExecutionTestFromFile("S15.8.2.2_A1");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.2_A2")]
    public Task S15_8_2_2_A2()
        => ExecutionTestFromFile("S15.8.2.2_A2");

    [Fact(DisplayName = "S15.8.2.2_A3")]
    public Task S15_8_2_2_A3()
        => ExecutionTestFromFile("S15.8.2.2_A3");

    [Fact(DisplayName = "S15.8.2.2_A4")]
    public Task S15_8_2_2_A4()
        => ExecutionTestFromFile("S15.8.2.2_A4");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
