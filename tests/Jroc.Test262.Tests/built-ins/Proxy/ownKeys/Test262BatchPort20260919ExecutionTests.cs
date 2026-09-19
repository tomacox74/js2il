using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.ownKeys;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.ownKeys") { }

    [Fact(DisplayName = "call-parameters-object-getownpropertynames")]
    public Task call_parameters_object_getownpropertynames()
        => ExecutionTestFromFile("call-parameters-object-getownpropertynames");

    [Fact(DisplayName = "call-parameters-object-getownpropertysymbols")]
    public Task call_parameters_object_getownpropertysymbols()
        => ExecutionTestFromFile("call-parameters-object-getownpropertysymbols");

    [Fact(DisplayName = "call-parameters-object-keys")]
    public Task call_parameters_object_keys()
        => ExecutionTestFromFile("call-parameters-object-keys");

    [Fact(DisplayName = "extensible-return-trap-result-absent-not-configurable-keys")]
    public Task extensible_return_trap_result_absent_not_configurable_keys()
        => ExecutionTestFromFile("extensible-return-trap-result-absent-not-configurable-keys");

    [Fact(DisplayName = "extensible-return-trap-result")]
    public Task extensible_return_trap_result()
        => ExecutionTestFromFile("extensible-return-trap-result");

    [Fact(DisplayName = "not-extensible-return-keys")]
    public Task not_extensible_return_keys()
        => ExecutionTestFromFile("not-extensible-return-keys");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-not-list-object-throws")]
    public Task return_not_list_object_throws()
        => ExecutionTestFromFile("return-not-list-object-throws");

    [Fact(DisplayName = "return-type-throws-array")]
    public Task return_type_throws_array()
        => ExecutionTestFromFile("return-type-throws-array");

    [Fact(DisplayName = "return-type-throws-boolean")]
    public Task return_type_throws_boolean()
        => ExecutionTestFromFile("return-type-throws-boolean");

    [Fact(DisplayName = "return-type-throws-null")]
    public Task return_type_throws_null()
        => ExecutionTestFromFile("return-type-throws-null");

    [Fact(DisplayName = "return-type-throws-number")]
    public Task return_type_throws_number()
        => ExecutionTestFromFile("return-type-throws-number");

    [Fact(DisplayName = "return-type-throws-object")]
    public Task return_type_throws_object()
        => ExecutionTestFromFile("return-type-throws-object");

    [Fact(DisplayName = "return-type-throws-undefined")]
    public Task return_type_throws_undefined()
        => ExecutionTestFromFile("return-type-throws-undefined");

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

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
