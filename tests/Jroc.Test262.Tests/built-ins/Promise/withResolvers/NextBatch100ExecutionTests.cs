using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.withResolvers;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.withResolvers") { }

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "ctx-non-object")]
    public Task ctx_non_object() => ExecutionTestFromFile("ctx-non-object");

    [Fact(DisplayName = "promise")]
    public Task promise() => ExecutionTestFromFile("promise");

    [Fact(DisplayName = "resolvers")]
    public Task resolvers() => ExecutionTestFromFile("resolvers");

    [Fact(DisplayName = "result")]
    public Task result() => ExecutionTestFromFile("result");
}
