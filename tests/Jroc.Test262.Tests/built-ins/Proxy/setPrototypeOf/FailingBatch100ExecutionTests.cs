using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.setPrototypeOf;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.setPrototypeOf") { }

    [Fact(DisplayName = "internals-call-order")]
    public Task internals_call_order() => ExecutionTestFromFile("internals-call-order");

    [Fact(DisplayName = "not-extensible-target-not-same-target-prototype")]
    public Task not_extensible_target_not_same_target_prototype() => ExecutionTestFromFile("not-extensible-target-not-same-target-prototype");

    [Fact(DisplayName = "return-abrupt-from-isextensible-target")]
    public Task return_abrupt_from_isextensible_target() => ExecutionTestFromFile("return-abrupt-from-isextensible-target");

    [Fact(DisplayName = "return-abrupt-from-target-getprototypeof")]
    public Task return_abrupt_from_target_getprototypeof() => ExecutionTestFromFile("return-abrupt-from-target-getprototypeof");

    [Fact(DisplayName = "toboolean-trap-result-false")]
    public Task toboolean_trap_result_false() => ExecutionTestFromFile("toboolean-trap-result-false");

    [Fact(DisplayName = "toboolean-trap-result-true-target-is-extensible")]
    public Task toboolean_trap_result_true_target_is_extensible() => ExecutionTestFromFile("toboolean-trap-result-true-target-is-extensible");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-or-null")]
    public Task trap_is_undefined_or_null() => ExecutionTestFromFile("trap-is-undefined-or-null");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy() => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

}
