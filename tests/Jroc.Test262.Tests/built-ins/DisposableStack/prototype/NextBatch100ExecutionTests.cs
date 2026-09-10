using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype") { }

    [Fact(DisplayName = "Symbol.dispose")]
    public Task Symbol_dispose() => ExecutionTestFromFile("Symbol.dispose");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag() => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "constructor")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto() => ExecutionTestFromFile("proto");
}
