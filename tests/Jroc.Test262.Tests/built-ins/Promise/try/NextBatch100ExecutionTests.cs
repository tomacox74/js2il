using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.try_;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.try") { }

    [Fact(DisplayName = "ctx-ctor-throws")]
    public Task ctx_ctor_throws() => ExecutionTestFromFile("ctx-ctor-throws");

    [Fact(DisplayName = "ctx-ctor")]
    public Task ctx_ctor() => ExecutionTestFromFile("ctx-ctor");

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "ctx-non-object")]
    public Task ctx_non_object() => ExecutionTestFromFile("ctx-non-object");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "promise")]
    public Task promise() => ExecutionTestFromFile("promise");
}
