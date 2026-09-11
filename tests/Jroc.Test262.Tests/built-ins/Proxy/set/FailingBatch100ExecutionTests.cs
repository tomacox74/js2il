using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.set;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.set") { }

    [Fact(DisplayName = "call-parameters-prototype-dunder-proto")]
    public Task call_parameters_prototype_dunder_proto() => ExecutionTestFromFile("call-parameters-prototype-dunder-proto");

    [Fact(DisplayName = "call-parameters-prototype-index")]
    public Task call_parameters_prototype_index() => ExecutionTestFromFile("call-parameters-prototype-index");

    [Fact(DisplayName = "call-parameters-prototype")]
    public Task call_parameters_prototype() => ExecutionTestFromFile("call-parameters-prototype");

    [Fact(DisplayName = "target-property-is-accessor-not-configurable-set-is-undefined")]
    public Task target_property_is_accessor_not_configurable_set_is_undefined() => ExecutionTestFromFile("target-property-is-accessor-not-configurable-set-is-undefined");

    [Fact(DisplayName = "target-property-is-not-configurable-not-writable-not-equal-to-v")]
    public Task target_property_is_not_configurable_not_writable_not_equal_to_v() => ExecutionTestFromFile("target-property-is-not-configurable-not-writable-not-equal-to-v");

    [Fact(DisplayName = "trap-is-missing-receiver-multiple-calls-index")]
    public Task trap_is_missing_receiver_multiple_calls_index() => ExecutionTestFromFile("trap-is-missing-receiver-multiple-calls-index");

    [Fact(DisplayName = "trap-is-missing-receiver-multiple-calls")]
    public Task trap_is_missing_receiver_multiple_calls() => ExecutionTestFromFile("trap-is-missing-receiver-multiple-calls");

    [Fact(DisplayName = "trap-is-null-receiver")]
    public Task trap_is_null_receiver() => ExecutionTestFromFile("trap-is-null-receiver");

}
