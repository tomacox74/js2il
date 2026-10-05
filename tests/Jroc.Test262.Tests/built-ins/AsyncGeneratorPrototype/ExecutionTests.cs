using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorPrototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");
}
