using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.EvalError.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.NativeErrors.EvalError.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "message")]
    public Task message()
        => ExecutionTestFromFile("message");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

}
