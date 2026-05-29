using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArrayConstructors.Uint8Array;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.Uint8Array") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

}
