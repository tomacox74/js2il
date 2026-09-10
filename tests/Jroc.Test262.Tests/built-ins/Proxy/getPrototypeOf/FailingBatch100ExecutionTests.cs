using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.getPrototypeOf;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.getPrototypeOf") { }

    [Fact(DisplayName = "instanceof-custom-return-accepted")]
    public Task instanceof_custom_return_accepted() => ExecutionTestFromFile("instanceof-custom-return-accepted");

    [Fact(DisplayName = "instanceof-target-not-extensible-not-same-proto-throws")]
    public Task instanceof_target_not_extensible_not_same_proto_throws() => ExecutionTestFromFile("instanceof-target-not-extensible-not-same-proto-throws");

    [Fact(DisplayName = "not-extensible-not-same-proto-throws")]
    public Task not_extensible_not_same_proto_throws() => ExecutionTestFromFile("not-extensible-not-same-proto-throws");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy() => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

}
