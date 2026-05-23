using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Set.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.delete") { }

    [Fact(DisplayName = "delete-entry")]
    public Task delete_entry()
        => ExecutionTestFromFile("delete-entry");

    [Fact(DisplayName = "returns-false-when-delete-is-noop")]
    public Task returns_false_when_delete_is_noop()
        => ExecutionTestFromFile("returns-false-when-delete-is-noop");
}
