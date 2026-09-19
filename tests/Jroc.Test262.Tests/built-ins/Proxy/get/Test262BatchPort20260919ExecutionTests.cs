using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.get;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.get") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-trap-result-accessor-property")]
    public Task return_trap_result_accessor_property()
        => ExecutionTestFromFile("return-trap-result-accessor-property");

    [Fact(DisplayName = "return-trap-result-configurable-false-writable-true")]
    public Task return_trap_result_configurable_false_writable_true()
        => ExecutionTestFromFile("return-trap-result-configurable-false-writable-true");

    [Fact(DisplayName = "return-trap-result-configurable-true-assessor-get-undefined")]
    public Task return_trap_result_configurable_true_assessor_get_undefined()
        => ExecutionTestFromFile("return-trap-result-configurable-true-assessor-get-undefined");

    [Fact(DisplayName = "return-trap-result-configurable-true-writable-false")]
    public Task return_trap_result_configurable_true_writable_false()
        => ExecutionTestFromFile("return-trap-result-configurable-true-writable-false");

    [Fact(DisplayName = "return-trap-result-same-value-configurable-false-writable-false")]
    public Task return_trap_result_same_value_configurable_false_writable_false()
        => ExecutionTestFromFile("return-trap-result-same-value-configurable-false-writable-false");

    [Fact(DisplayName = "return-trap-result")]
    public Task return_trap_result()
        => ExecutionTestFromFile("return-trap-result");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-undefined-no-property")]
    public Task trap_is_undefined_no_property()
        => ExecutionTestFromFile("trap-is-undefined-no-property");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
