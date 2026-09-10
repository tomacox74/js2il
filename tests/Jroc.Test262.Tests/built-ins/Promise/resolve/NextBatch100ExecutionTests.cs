using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.resolve;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.resolve") { }

    [Fact(DisplayName = "S25.4.4.5_A2.1_T1")]
    public Task S25_4_4_5_A2_1_T1() => ExecutionTestFromFile("S25.4.4.5_A2.1_T1");

    [Fact(DisplayName = "capability-executor-called-twice")]
    public Task capability_executor_called_twice() => ExecutionTestFromFile("capability-executor-called-twice");

    [Fact(DisplayName = "capability-executor-not-callable")]
    public Task capability_executor_not_callable() => ExecutionTestFromFile("capability-executor-not-callable");

    [Fact(DisplayName = "capability-invocation-error")]
    public Task capability_invocation_error() => ExecutionTestFromFile("capability-invocation-error");

    [Fact(DisplayName = "ctx-ctor-throws")]
    public Task ctx_ctor_throws() => ExecutionTestFromFile("ctx-ctor-throws");

    [Fact(DisplayName = "ctx-ctor")]
    public Task ctx_ctor() => ExecutionTestFromFile("ctx-ctor");

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "resolve-from-promise-capability")]
    public Task resolve_from_promise_capability() => ExecutionTestFromFile("resolve-from-promise-capability");

    [Fact(DisplayName = "resolve-prms-cstm-then")]
    public Task resolve_prms_cstm_then() => ExecutionTestFromFile("resolve-prms-cstm-then");
}
