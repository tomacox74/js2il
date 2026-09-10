using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.reject;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.reject") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "capability-executor-called-twice")]
    public Task capability_executor_called_twice() => ExecutionTestFromFile("capability-executor-called-twice");

    [Fact(DisplayName = "capability-executor-not-callable")]
    public Task capability_executor_not_callable() => ExecutionTestFromFile("capability-executor-not-callable");

    [Fact(DisplayName = "capability-invocation-error")]
    public Task capability_invocation_error() => ExecutionTestFromFile("capability-invocation-error");

    [Fact(DisplayName = "capability-invocation")]
    public Task capability_invocation() => ExecutionTestFromFile("capability-invocation");

    [Fact(DisplayName = "ctx-ctor")]
    public Task ctx_ctor() => ExecutionTestFromFile("ctx-ctor");
}
