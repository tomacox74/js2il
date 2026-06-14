using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.delete;

public class PortAdditionalExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.WeakSet.prototype.delete") { }

    [Fact(DisplayName = "returns-false-when-value-cannot-be-held-weakly")]
    public Task returns_false_when_value_cannot_be_held_weakly()
        => ExecutionTestFromFile("returns-false-when-value-cannot-be-held-weakly");

}
