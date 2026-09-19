using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.set;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Proxy.set") { }

    [Fact(DisplayName = "boolean-trap-result-is-false-boolean-return-false")]
    public Task boolean_trap_result_is_false_boolean_return_false()
        => ExecutionTestFromFile("boolean-trap-result-is-false-boolean-return-false");

    [Fact(DisplayName = "boolean-trap-result-is-false-null-return-false")]
    public Task boolean_trap_result_is_false_null_return_false()
        => ExecutionTestFromFile("boolean-trap-result-is-false-null-return-false");

    [Fact(DisplayName = "boolean-trap-result-is-false-number-return-false")]
    public Task boolean_trap_result_is_false_number_return_false()
        => ExecutionTestFromFile("boolean-trap-result-is-false-number-return-false");

    [Fact(DisplayName = "boolean-trap-result-is-false-string-return-false")]
    public Task boolean_trap_result_is_false_string_return_false()
        => ExecutionTestFromFile("boolean-trap-result-is-false-string-return-false");

    [Fact(DisplayName = "boolean-trap-result-is-false-undefined-return-false")]
    public Task boolean_trap_result_is_false_undefined_return_false()
        => ExecutionTestFromFile("boolean-trap-result-is-false-undefined-return-false");

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-true-target-property-accessor-is-configurable-set-is-undefined")]
    public Task return_true_target_property_accessor_is_configurable_set_is_undefined()
        => ExecutionTestFromFile("return-true-target-property-accessor-is-configurable-set-is-undefined");

    [Fact(DisplayName = "return-true-target-property-accessor-is-not-configurable")]
    public Task return_true_target_property_accessor_is_not_configurable()
        => ExecutionTestFromFile("return-true-target-property-accessor-is-not-configurable");

    [Fact(DisplayName = "return-true-target-property-is-not-configurable")]
    public Task return_true_target_property_is_not_configurable()
        => ExecutionTestFromFile("return-true-target-property-is-not-configurable");

    [Fact(DisplayName = "return-true-target-property-is-not-writable")]
    public Task return_true_target_property_is_not_writable()
        => ExecutionTestFromFile("return-true-target-property-is-not-writable");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy()
        => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy()
        => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-no-property")]
    public Task trap_is_undefined_no_property()
        => ExecutionTestFromFile("trap-is-undefined-no-property");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy()
        => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
