using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.preventExtensions;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.preventExtensions") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-true-target-is-extensible")]
    public Task return_true_target_is_extensible()
        => ExecutionTestFromFile("return-true-target-is-extensible");

}
