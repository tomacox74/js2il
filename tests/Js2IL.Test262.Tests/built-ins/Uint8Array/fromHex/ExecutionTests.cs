using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Uint8Array.fromHex;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.fromHex") { }

    [Fact(DisplayName = "Uint8Array_fromHex_results")]
    public Task Uint8Array_fromHex_results()
        => ExecutionTest("Uint8Array_fromHex_results");
}
