using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.isExtensible;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.isExtensible") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-is-boolean")]
    public Task return_is_boolean()
        => ExecutionTestFromFile("return-is-boolean");

    [Fact(DisplayName = "return-is-different-from-target")]
    public Task return_is_different_from_target()
        => ExecutionTestFromFile("return-is-different-from-target");

    [Fact(DisplayName = "return-same-result-from-target")]
    public Task return_same_result_from_target()
        => ExecutionTestFromFile("return-same-result-from-target");

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
