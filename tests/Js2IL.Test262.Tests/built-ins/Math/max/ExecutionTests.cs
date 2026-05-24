using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Math.max;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Math.max") { }



    [Fact(DisplayName = "15.8.2.11-1")]

    public Task _15_8_2_11_1()

        => ExecutionTestFromFile("15.8.2.11-1");



    [Fact(DisplayName = "Math.max_each-element-coerced")]

    public Task Math_max_each_element_coerced()

        => ExecutionTestFromFile("Math.max_each-element-coerced");



    [Fact(DisplayName = "S15.8.2.11_A1")]

    public Task S15_8_2_11_A1()

        => ExecutionTestFromFile("S15.8.2.11_A1");



    [Fact(DisplayName = "S15.8.2.11_A2")]

    public Task S15_8_2_11_A2()

        => ExecutionTestFromFile("S15.8.2.11_A2");



    [Fact(DisplayName = "zeros")]

    public Task zeros()

        => ExecutionTestFromFile("zeros");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.8.2.11_A4")]
    public Task S15_8_2_11_A4()
        => ExecutionTestFromFile("S15.8.2.11_A4");

}
