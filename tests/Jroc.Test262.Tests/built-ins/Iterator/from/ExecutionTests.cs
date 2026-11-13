using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.from;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator.from") { }

    [Fact(DisplayName = "get-return-method-when-call-return.js")]
    public Task Port200_get_return_method_when_call_return()
        => ExecutionTestFromFile("get-return-method-when-call-return");

    [Fact(DisplayName = "return-method-calls-base-return-method.js")]
    public Task Port200_return_method_calls_base_return_method()
        => ExecutionTestFromFile("return-method-calls-base-return-method");

    [Fact(DisplayName = "return-method-throws-for-invalid-this.js")]
    public Task Port200_return_method_throws_for_invalid_this()
        => ExecutionTestFromFile("return-method-throws-for-invalid-this");
}
