using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.slice;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "extensible")]
    public Task extensible()
        => ExecutionTestFromFile("extensible");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

}
