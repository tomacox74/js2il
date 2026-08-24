using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number.prototype.toString") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.7.4.2_A1_T01")]
    public Task S15_7_4_2_A1_T01()
        => ExecutionTestFromFile("S15.7.4.2_A1_T01");

    [Fact(DisplayName = "a-z")]
    public Task a_z()
        => ExecutionTestFromFile("a-z");

    [Fact(DisplayName = "numeric-literal-tostring-radix-1")]
    public Task numeric_literal_tostring_radix_1()
        => ExecutionTestFromFile("numeric-literal-tostring-radix-1");

    [Fact(DisplayName = "numeric-literal-tostring-radix-37")]
    public Task numeric_literal_tostring_radix_37()
        => ExecutionTestFromFile("numeric-literal-tostring-radix-37");

    [Fact(DisplayName = "numeric-literal-tostring-radix-poisoned")]
    public Task numeric_literal_tostring_radix_poisoned()
        => ExecutionTestFromFile("numeric-literal-tostring-radix-poisoned");
}
