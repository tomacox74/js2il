using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakMap.prototype.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.get") { }

    [Fact(DisplayName = "returns-value-with-object-key", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task returns_value_with_object_key()
        => ExecutionTestFromFile("returns-value-with-object-key");
}
