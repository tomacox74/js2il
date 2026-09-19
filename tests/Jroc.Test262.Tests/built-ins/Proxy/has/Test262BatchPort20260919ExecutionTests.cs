using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.has;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.has") { }

    [Fact(DisplayName = "call-in")]
    public Task call_in()
        => ExecutionTestFromFile("call-in");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-false-target-prop-exists-using-with")]
    public Task return_false_target_prop_exists_using_with()
        => ExecutionTestFromFile("return-false-target-prop-exists-using-with");

    [Fact(DisplayName = "return-false-target-prop-exists")]
    public Task return_false_target_prop_exists()
        => ExecutionTestFromFile("return-false-target-prop-exists");

    [Fact(DisplayName = "return-is-abrupt-in")]
    public Task return_is_abrupt_in()
        => ExecutionTestFromFile("return-is-abrupt-in");

    [Fact(DisplayName = "return-true-target-prop-exists-using-with")]
    public Task return_true_target_prop_exists_using_with()
        => ExecutionTestFromFile("return-true-target-prop-exists-using-with");

    [Fact(DisplayName = "return-true-target-prop-exists")]
    public Task return_true_target_prop_exists()
        => ExecutionTestFromFile("return-true-target-prop-exists");

    [Fact(DisplayName = "return-true-without-same-target-prop")]
    public Task return_true_without_same_target_prop()
        => ExecutionTestFromFile("return-true-without-same-target-prop");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-undefined-target-is-proxy")]
    public Task trap_is_undefined_target_is_proxy()
        => ExecutionTestFromFile("trap-is-undefined-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
