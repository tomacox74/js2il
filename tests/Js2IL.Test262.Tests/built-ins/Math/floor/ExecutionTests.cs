using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Math.floor;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Math.floor") { }



    [Fact(DisplayName = "S15.8.2.9_A1")]

    public Task S15_8_2_9_A1()

        => ExecutionTestFromFile("S15.8.2.9_A1");



    [Fact(DisplayName = "S15.8.2.9_A2")]

    public Task S15_8_2_9_A2()

        => ExecutionTestFromFile("S15.8.2.9_A2");



    [Fact(DisplayName = "S15.8.2.9_A3")]

    public Task S15_8_2_9_A3()

        => ExecutionTestFromFile("S15.8.2.9_A3");



    [Fact(DisplayName = "S15.8.2.9_A4")]

    public Task S15_8_2_9_A4()

        => ExecutionTestFromFile("S15.8.2.9_A4");



    [Fact(DisplayName = "S15.8.2.9_A6")]

    public Task S15_8_2_9_A6()

        => ExecutionTestFromFile("S15.8.2.9_A6");

    [Fact(DisplayName = "length", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.9_A5")]
    public Task S15_8_2_9_A5()
        => ExecutionTestFromFile("S15.8.2.9_A5");

}
