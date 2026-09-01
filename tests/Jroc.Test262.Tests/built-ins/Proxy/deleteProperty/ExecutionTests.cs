using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.deleteProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy.deleteProperty") { }

    [Fact(DisplayName = "boolean-trap-result-boolean-false")]
    public Task boolean_trap_result_boolean_false() => ExecutionTestFromFile("boolean-trap-result-boolean-false");

    [Fact(DisplayName = "boolean-trap-result-boolean-true")]
    public Task boolean_trap_result_boolean_true() => ExecutionTestFromFile("boolean-trap-result-boolean-true");

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters() => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler() => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-false-not-strict")]
    public Task return_false_not_strict() => ExecutionTestFromFile("return-false-not-strict");

    [Fact(DisplayName = "return-false-strict")]
    public Task return_false_strict() => ExecutionTestFromFile("return-false-strict");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt() => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "targetdesc-is-configurable-target-is-not-extensible")]
    public Task targetdesc_is_configurable_target_is_not_extensible() => ExecutionTestFromFile("targetdesc-is-configurable-target-is-not-extensible");

    [Fact(DisplayName = "targetdesc-is-not-configurable")]
    public Task targetdesc_is_not_configurable() => ExecutionTestFromFile("targetdesc-is-not-configurable");

    [Fact(DisplayName = "targetdesc-is-undefined-return-true")]
    public Task targetdesc_is_undefined_return_true() => ExecutionTestFromFile("targetdesc-is-undefined-return-true");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable() => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-not-strict")]
    public Task trap_is_undefined_not_strict() => ExecutionTestFromFile("trap-is-undefined-not-strict");

    [Fact(DisplayName = "trap-is-undefined-strict")]
    public Task trap_is_undefined_strict() => ExecutionTestFromFile("trap-is-undefined-strict");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy() => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");
}
