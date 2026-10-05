using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorPrototype") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag() => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "constructor")]
    public Task constructor() => ExecutionTestFromFile("constructor");
}
