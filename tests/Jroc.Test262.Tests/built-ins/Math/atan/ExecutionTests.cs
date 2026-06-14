using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.atan;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.atan") { }

    [Fact(DisplayName = "S15.8.2.4_A1")]
    public Task S15_8_2_4_A1()
        => ExecutionTestFromFile("S15.8.2.4_A1");

    [Fact(DisplayName = "S15.8.2.4_A2")]
    public Task S15_8_2_4_A2()
        => ExecutionTestFromFile("S15.8.2.4_A2");

    [Fact(DisplayName = "S15.8.2.4_A3")]
    public Task S15_8_2_4_A3()
        => ExecutionTestFromFile("S15.8.2.4_A3");

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
