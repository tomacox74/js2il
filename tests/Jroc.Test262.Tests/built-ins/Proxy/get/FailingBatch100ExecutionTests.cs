using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.get;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.get") { }

    [Fact(DisplayName = "accessor-get-is-undefined-throws")]
    public Task accessor_get_is_undefined_throws() => ExecutionTestFromFile("accessor-get-is-undefined-throws");

    [Fact(DisplayName = "not-same-value-configurable-false-writable-false-throws")]
    public Task not_same_value_configurable_false_writable_false_throws() => ExecutionTestFromFile("not-same-value-configurable-false-writable-false-throws");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-receiver")]
    public Task trap_is_undefined_receiver() => ExecutionTestFromFile("trap-is-undefined-receiver");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy() => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

}
