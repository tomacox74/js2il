using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.internals.Construct;

public sealed class ConstructorCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ConstructorCompletionExecutionTests() : base("Class.ConstructorCompletion") { }

    [Fact(DisplayName = "derived-this-uninitialized.js")]
    public Task derived_this_uninitialized()
        => ExecutionTestFromFile("derived-this-uninitialized");

}
