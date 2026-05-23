using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Math.abs;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.abs") { }

    [Fact(DisplayName = "absolute-value")]
    public Task absolute_value()
        => ExecutionTest("absolute-value");

    [Fact(DisplayName = "S15.8.2.1_A1")]
    public Task S15_8_2_1_A1()
        => ExecutionTest("S15.8.2.1_A1");

    [Fact(DisplayName = "S15.8.2.1_A2")]
    public Task S15_8_2_1_A2()
        => ExecutionTest("S15.8.2.1_A2");

    [Fact(DisplayName = "S15.8.2.1_A3")]
    public Task S15_8_2_1_A3()
        => ExecutionTest("S15.8.2.1_A3");

    [Fact(DisplayName = "length", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

}
