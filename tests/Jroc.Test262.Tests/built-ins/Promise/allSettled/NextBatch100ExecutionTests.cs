using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettled;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.allSettled") { }

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "ctx-non-object")]
    public Task ctx_non_object() => ExecutionTestFromFile("ctx-non-object");

    [Fact(DisplayName = "returns-promise")]
    public Task returns_promise() => ExecutionTestFromFile("returns-promise");
}
