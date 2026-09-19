using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.isError;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Error.isError") { }

    [Fact(DisplayName = "bigints")]
    public Task bigints()
        => ExecutionTestFromFile("bigints");

    [Fact(DisplayName = "errors")]
    public Task errors()
        => ExecutionTestFromFile("errors");

    [Fact(DisplayName = "fake-errors")]
    public Task fake_errors()
        => ExecutionTestFromFile("fake-errors");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "non-error-objects")]
    public Task non_error_objects()
        => ExecutionTestFromFile("non-error-objects");

    [Fact(DisplayName = "primitives")]
    public Task primitives()
        => ExecutionTestFromFile("primitives");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "symbols")]
    public Task symbols()
        => ExecutionTestFromFile("symbols");

}
