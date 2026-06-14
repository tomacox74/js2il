using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.isNaN;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number.isNaN") { }

    [Fact(DisplayName = "arg-is-not-number")]
    public Task arg_is_not_number()
        => ExecutionTestFromFile("arg-is-not-number");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nan")]
    public Task nan()
        => ExecutionTestFromFile("nan");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "not-nan")]
    public Task not_nan()
        => ExecutionTestFromFile("not-nan");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");
}
