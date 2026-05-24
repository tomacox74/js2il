using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Math.acos;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.acos") { }

    [Fact(DisplayName = "S15.8.2.2_A1")]
    public Task S15_8_2_2_A1()
        => ExecutionTest("S15.8.2.2_A1");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

    [Fact(DisplayName = "S15.8.2.2_A2")]
    public Task S15_8_2_2_A2()
        => ExecutionTest("S15.8.2.2_A2");

}
