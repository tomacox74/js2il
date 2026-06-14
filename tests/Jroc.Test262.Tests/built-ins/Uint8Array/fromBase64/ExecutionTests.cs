using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.fromBase64;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.fromBase64") { }

    [Fact(DisplayName = "Uint8Array_fromBase64_results")]
    public Task Uint8Array_fromBase64_results()
        => ExecutionTest("Uint8Array_fromBase64_results");

    [Fact(DisplayName = "string-coercion")]
    public Task string_coercion()
        => ExecutionTest("string-coercion");
    [Fact(DisplayName = "illegal-characters")]
    public Task illegal_characters()
        => ExecutionTest("illegal-characters");

}
