using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.toHex;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.prototype.toHex") { }

    [Fact(DisplayName = "results")]
    public Task results()
        => ExecutionTest("results");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");
}
