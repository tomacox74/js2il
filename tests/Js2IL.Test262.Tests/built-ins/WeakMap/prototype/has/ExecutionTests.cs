using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakMap.prototype.has;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.has") { }

    [Fact(DisplayName = "returns-true-when-object-key-present")]
    public Task returns_true_when_object_key_present()
        => ExecutionTestFromFile("returns-true-when-object-key-present");
}
