using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.SyntaxError.prototype;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.NativeErrors.SyntaxError.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "message")]
    public Task message()
        => ExecutionTestFromFile("message");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-error-object")]
    public Task not_error_object()
        => ExecutionTestFromFile("not-error-object");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

}
