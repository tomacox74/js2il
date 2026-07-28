using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.setFromHex;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.prototype.setFromHex") { }

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTest("descriptor");

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

    [Fact(DisplayName = "subarray")]
    public Task subarray()
        => ExecutionTest("subarray");

    [Fact(DisplayName = "target-size")]
    public Task target_size()
        => ExecutionTest("target-size");

    [Fact(DisplayName = "throws-when-string-length-is-odd")]
    public Task throws_when_string_length_is_odd()
        => ExecutionTest("throws-when-string-length-is-odd");

    [Fact(DisplayName = "writes-up-to-error")]
    public Task writes_up_to_error()
        => ExecutionTest("writes-up-to-error");
}
