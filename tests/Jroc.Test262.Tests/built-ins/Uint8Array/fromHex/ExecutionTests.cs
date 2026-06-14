using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.fromHex;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.fromHex") { }

    [Fact(DisplayName = "Uint8Array_fromHex_results")]
    public Task Uint8Array_fromHex_results()
        => ExecutionTest("Uint8Array_fromHex_results");

    [Fact(DisplayName = "string-coercion")]
    public Task string_coercion()
        => ExecutionTest("string-coercion");
    [Fact(DisplayName = "odd-length-input")]
    public Task odd_length_input()
        => ExecutionTest("odd-length-input");

}
