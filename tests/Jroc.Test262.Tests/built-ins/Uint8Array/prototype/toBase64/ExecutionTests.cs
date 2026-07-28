using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.toBase64;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.prototype.toBase64") { }

    [Fact(DisplayName = "alphabet")]
    public Task alphabet()
        => ExecutionTest("alphabet");

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTest("descriptor");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "nonconstructor")]
    public Task nonconstructor()
        => ExecutionTest("nonconstructor");

    [Fact(DisplayName = "omit-padding")]
    public Task omit_padding()
        => ExecutionTest("omit-padding");

    [Fact(DisplayName = "option-coercion")]
    public Task option_coercion()
        => ExecutionTest("option-coercion");

    [Fact(DisplayName = "results")]
    public Task results()
        => ExecutionTest("results");
}
