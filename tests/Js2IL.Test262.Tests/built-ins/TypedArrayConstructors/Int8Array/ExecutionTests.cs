using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArrayConstructors.Int8Array;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArrayConstructors.Int8Array") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");
}
