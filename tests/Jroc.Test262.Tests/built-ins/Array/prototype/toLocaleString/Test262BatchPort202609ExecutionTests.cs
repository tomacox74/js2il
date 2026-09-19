using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toLocaleString;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.toLocaleString") { }

    [Fact(DisplayName = "S15.4.4.3_A1_T1")]
    public Task S15_4_4_3_A1_T1()
        => ExecutionTestFromFile("S15.4.4.3_A1_T1");

    [Fact(DisplayName = "S15.4.4.3_A3_T1")]
    public Task S15_4_4_3_A3_T1()
        => ExecutionTestFromFile("S15.4.4.3_A3_T1");

    [Fact(DisplayName = "invoke-element-tolocalestring")]
    public Task invoke_element_tolocalestring()
        => ExecutionTestFromFile("invoke-element-tolocalestring");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "primitive_this_value")]
    public Task primitive_this_value()
        => ExecutionTestFromFile("primitive_this_value");

    [Fact(DisplayName = "primitive_this_value_getter")]
    public Task primitive_this_value_getter()
        => ExecutionTestFromFile("primitive_this_value_getter");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
