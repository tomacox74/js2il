using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.preventExtensions;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Proxy.preventExtensions") { }

    [Fact(DisplayName = "return-true-target-is-not-extensible")]
    public Task return_true_target_is_not_extensible()
        => ExecutionTestFromFile("return-true-target-is-not-extensible");

    [Fact(DisplayName = "trap-is-missing-target-is-proxy")]
    public Task trap_is_missing_target_is_proxy()
        => ExecutionTestFromFile("trap-is-missing-target-is-proxy");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null-target-is-proxy")]
    public Task trap_is_null_target_is_proxy()
        => ExecutionTestFromFile("trap-is-null-target-is-proxy");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
