using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArrayConstructors.Uint8ClampedArray;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.Uint8ClampedArray") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

}
