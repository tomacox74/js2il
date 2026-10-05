using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorFunction.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorFunction.prototype") { }

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prototype")]
    public Task prototype()
        => ExecutionTestFromFile("prototype");
}
