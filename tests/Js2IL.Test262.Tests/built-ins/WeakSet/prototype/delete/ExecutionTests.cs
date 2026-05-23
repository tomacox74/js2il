using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet.prototype.delete") { }

    [Fact(DisplayName = "delete-object-entry")]
    public Task delete_object_entry()
        => ExecutionTestFromFile("delete-object-entry");

    [Fact(DisplayName = "returns-false-when-delete-is-noop")]
    public Task returns_false_when_delete_is_noop()
        => ExecutionTestFromFile("returns-false-when-delete-is-noop");
}
