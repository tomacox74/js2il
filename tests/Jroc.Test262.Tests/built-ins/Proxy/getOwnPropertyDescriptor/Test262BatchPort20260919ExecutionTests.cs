using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.getOwnPropertyDescriptor;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "result-is-undefined-targetdesc-is-not-configurable")]
    public Task result_is_undefined_targetdesc_is_not_configurable()
        => ExecutionTestFromFile("result-is-undefined-targetdesc-is-not-configurable");

    [Fact(DisplayName = "result-is-undefined-targetdesc-is-undefined")]
    public Task result_is_undefined_targetdesc_is_undefined()
        => ExecutionTestFromFile("result-is-undefined-targetdesc-is-undefined");

    [Fact(DisplayName = "result-is-undefined")]
    public Task result_is_undefined()
        => ExecutionTestFromFile("result-is-undefined");

    [Fact(DisplayName = "result-type-is-not-object-nor-undefined")]
    public Task result_type_is_not_object_nor_undefined()
        => ExecutionTestFromFile("result-type-is-not-object-nor-undefined");

    [Fact(DisplayName = "resultdesc-is-not-configurable-not-writable-targetdesc-is-writable")]
    public Task resultdesc_is_not_configurable_not_writable_targetdesc_is_writable()
        => ExecutionTestFromFile("resultdesc-is-not-configurable-not-writable-targetdesc-is-writable");

    [Fact(DisplayName = "resultdesc-is-not-configurable-targetdesc-is-configurable")]
    public Task resultdesc_is_not_configurable_targetdesc_is_configurable()
        => ExecutionTestFromFile("resultdesc-is-not-configurable-targetdesc-is-configurable");

    [Fact(DisplayName = "resultdesc-is-not-configurable-targetdesc-is-undefined")]
    public Task resultdesc_is_not_configurable_targetdesc_is_undefined()
        => ExecutionTestFromFile("resultdesc-is-not-configurable-targetdesc-is-undefined");

    [Fact(DisplayName = "resultdesc-return-configurable")]
    public Task resultdesc_return_configurable()
        => ExecutionTestFromFile("resultdesc-return-configurable");

    [Fact(DisplayName = "resultdesc-return-not-configurable")]
    public Task resultdesc_return_not_configurable()
        => ExecutionTestFromFile("resultdesc-return-not-configurable");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy()
        => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy()
        => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy()
        => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

}
