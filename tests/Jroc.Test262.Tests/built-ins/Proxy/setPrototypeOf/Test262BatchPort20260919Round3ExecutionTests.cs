using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.setPrototypeOf;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Proxy.setPrototypeOf") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "not-extensible-target-same-target-prototype")]
    public Task not_extensible_target_same_target_prototype()
        => ExecutionTestFromFile("not-extensible-target-same-target-prototype");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-abrupt-from-get-trap")]
    public Task return_abrupt_from_get_trap()
        => ExecutionTestFromFile("return-abrupt-from-get-trap");

    [Fact(DisplayName = "return-abrupt-from-trap")]
    public Task return_abrupt_from_trap()
        => ExecutionTestFromFile("return-abrupt-from-trap");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

}
