using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "Symbol.iterator")]
    public Task Symbol_iterator()
        => ExecutionTestFromFile("Symbol.iterator");
}
