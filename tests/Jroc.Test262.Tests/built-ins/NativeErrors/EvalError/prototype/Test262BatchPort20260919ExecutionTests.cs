using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.EvalError.prototype;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.NativeErrors.EvalError.prototype") { }

    [Fact(DisplayName = "not-error-object")]
    public Task not_error_object()
        => ExecutionTestFromFile("not-error-object");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

}
