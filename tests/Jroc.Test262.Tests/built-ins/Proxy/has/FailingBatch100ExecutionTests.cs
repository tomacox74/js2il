using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.has;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.has") { }

    [Fact(DisplayName = "call-in-prototype-index")]
    public Task call_in_prototype_index() => ExecutionTestFromFile("call-in-prototype-index");

    [Fact(DisplayName = "call-in-prototype")]
    public Task call_in_prototype() => ExecutionTestFromFile("call-in-prototype");

    [Fact(DisplayName = "call-object-create")]
    public Task call_object_create() => ExecutionTestFromFile("call-object-create");

    [Fact(DisplayName = "call-with")]
    public Task call_with() => ExecutionTestFromFile("call-with");

    [Fact(DisplayName = "return-false-target-not-extensible")]
    public Task return_false_target_not_extensible() => ExecutionTestFromFile("return-false-target-not-extensible");

    [Fact(DisplayName = "return-false-targetdesc-not-configurable")]
    public Task return_false_targetdesc_not_configurable() => ExecutionTestFromFile("return-false-targetdesc-not-configurable");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy() => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy() => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined-using-with")]
    public Task trap_is_undefined_using_with() => ExecutionTestFromFile("trap-is-undefined-using-with");

}
