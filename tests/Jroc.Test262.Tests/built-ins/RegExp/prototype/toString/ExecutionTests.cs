using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.toString") { }

    [Fact(DisplayName = "S15.10.6.4_A10.js")]
    public Task S15_10_6_4_A10()
        => ExecutionTestFromFile("S15.10.6.4_A10");

    [Fact(DisplayName = "S15.10.6.4_A11.js")]
    public Task S15_10_6_4_A11()
        => ExecutionTestFromFile("S15.10.6.4_A11");

    [Fact(DisplayName = "S15.10.6.4_A6.js")]
    public Task S15_10_6_4_A6()
        => ExecutionTestFromFile("S15.10.6.4_A6");

    [Fact(DisplayName = "S15.10.6.4_A7.js")]
    public Task S15_10_6_4_A7()
        => ExecutionTestFromFile("S15.10.6.4_A7");

    [Fact(DisplayName = "S15.10.6.4_A8.js")]
    public Task S15_10_6_4_A8()
        => ExecutionTestFromFile("S15.10.6.4_A8");

    [Fact(DisplayName = "S15.10.6.4_A9.js")]
    public Task S15_10_6_4_A9()
        => ExecutionTestFromFile("S15.10.6.4_A9");

    [Fact(DisplayName = "called-as-function.js")]
    public Task called_as_function()
        => ExecutionTestFromFile("called-as-function");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
