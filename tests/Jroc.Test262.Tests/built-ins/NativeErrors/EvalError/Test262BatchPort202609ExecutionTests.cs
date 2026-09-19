using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.EvalError;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.NativeErrors.EvalError") { }

    [Fact(DisplayName = "is-error-object")]
    public Task is_error_object()
        => ExecutionTestFromFile("is-error-object");

}
