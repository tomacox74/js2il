using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype") { }

    [Fact(DisplayName = "Symbol.asyncDispose.js")]
    public Task Symbol_asyncDispose() => ExecutionTestFromFile("Symbol.asyncDispose");

    [Fact(DisplayName = "Symbol.toStringTag.js")]
    public Task Symbol_toStringTag() => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");
}
