using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.defineProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy.defineProperty") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters() => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler() => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-boolean-and-define-target")]
    public Task return_boolean_and_define_target() => ExecutionTestFromFile("return-boolean-and-define-target");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt() => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "targetdesc-configurable-desc-not-configurable")]
    public Task targetdesc_configurable_desc_not_configurable() => ExecutionTestFromFile("targetdesc-configurable-desc-not-configurable");

    [Fact(DisplayName = "targetdesc-not-compatible-descriptor")]
    public Task targetdesc_not_compatible_descriptor() => ExecutionTestFromFile("targetdesc-not-compatible-descriptor");

    [Fact(DisplayName = "targetdesc-not-compatible-descriptor-not-configurable-target")]
    public Task targetdesc_not_compatible_descriptor_not_configurable_target() => ExecutionTestFromFile("targetdesc-not-compatible-descriptor-not-configurable-target");

    [Fact(DisplayName = "targetdesc-not-configurable-writable-desc-not-writable")]
    public Task targetdesc_not_configurable_writable_desc_not_writable() => ExecutionTestFromFile("targetdesc-not-configurable-writable-desc-not-writable");

    [Fact(DisplayName = "targetdesc-undefined-not-configurable-descriptor")]
    public Task targetdesc_undefined_not_configurable_descriptor() => ExecutionTestFromFile("targetdesc-undefined-not-configurable-descriptor");

    [Fact(DisplayName = "targetdesc-undefined-target-is-not-extensible")]
    public Task targetdesc_undefined_target_is_not_extensible() => ExecutionTestFromFile("targetdesc-undefined-target-is-not-extensible");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable() => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined() => ExecutionTestFromFile("trap-is-undefined");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy() => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

    [Fact(DisplayName = "trap-return-is-false")]
    public Task trap_return_is_false() => ExecutionTestFromFile("trap-return-is-false");
}
