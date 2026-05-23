using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet.prototype.has;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet.prototype.has") { }

    [Fact(DisplayName = "returns-true-when-object-value-present")]
    public Task returns_true_when_object_value_present()
        => ExecutionTestFromFile("returns-true-when-object-value-present");
}
