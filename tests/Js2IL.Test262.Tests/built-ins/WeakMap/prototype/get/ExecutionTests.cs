using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakMap.prototype.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.get") { }

    [Fact(DisplayName = "returns-value-with-object-key")]
    public Task returns_value_with_object_key()
        => ExecutionTestFromFile("returns-value-with-object-key");
}
