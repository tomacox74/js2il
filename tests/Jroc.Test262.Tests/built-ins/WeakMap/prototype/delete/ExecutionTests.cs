using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.delete") { }

    [Fact(DisplayName = "delete-entry-with-object-key")]
    public Task delete_entry_with_object_key()
        => ExecutionTestFromFile("delete-entry-with-object-key");
}
