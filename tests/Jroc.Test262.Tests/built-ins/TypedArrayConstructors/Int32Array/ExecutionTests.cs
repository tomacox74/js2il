using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.Int32Array;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("TypedArrayConstructors.Int32Array") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");
}
