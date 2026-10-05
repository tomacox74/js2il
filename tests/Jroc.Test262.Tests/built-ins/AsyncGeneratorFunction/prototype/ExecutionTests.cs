using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorFunction.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction.prototype") { }

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "not-callable")]
    public Task not_callable()
        => ExecutionTestFromFile("not-callable");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "prototype")]
    public Task prototype()
        => ExecutionTestFromFile("prototype");
}
