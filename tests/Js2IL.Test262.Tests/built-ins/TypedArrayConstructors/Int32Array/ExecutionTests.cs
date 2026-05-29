using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArrayConstructors.Int32Array;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArrayConstructors.Int32Array") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");
}
