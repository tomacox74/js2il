using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.add;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet.prototype.add") { }

    [Fact(DisplayName = "adds-object-element")]
    public Task adds_object_element()
        => ExecutionTestFromFile("adds-object-element");

    [Fact(DisplayName = "returns-this")]
    public Task returns_this()
        => ExecutionTestFromFile("returns-this");
}
