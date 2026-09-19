using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.use;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.use") { }

    [Fact(DisplayName = "this-does-not-have-internal-asyncdisposablestate-throws")]
    public Task this_does_not_have_internal_asyncdisposablestate_throws()
        => ExecutionTestFromFile("this-does-not-have-internal-asyncdisposablestate-throws");

}
