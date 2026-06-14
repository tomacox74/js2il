using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.add;

public class PortAdditionalExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.WeakSet.prototype.add") { }

    [Fact(DisplayName = "add")]
    public Task add()
        => ExecutionTestFromFile("add");

    [Fact(DisplayName = "returns-this-when-ignoring-duplicate")]
    public Task returns_this_when_ignoring_duplicate()
        => ExecutionTestFromFile("returns-this-when-ignoring-duplicate");

}
