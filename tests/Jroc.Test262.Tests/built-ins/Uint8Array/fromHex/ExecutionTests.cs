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

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTest("descriptor");

    [Fact(DisplayName = "ignores-receiver")]
    public Task ignores_receiver()
        => ExecutionTest("ignores-receiver");

    [Fact(DisplayName = "illegal-characters")]
    public Task illegal_characters()
        => ExecutionTest("illegal-characters");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "nonconstructor")]
    public Task nonconstructor()
        => ExecutionTest("nonconstructor");

    [Fact(DisplayName = "results")]
    public Task results()
        => ExecutionTest("results");
}
