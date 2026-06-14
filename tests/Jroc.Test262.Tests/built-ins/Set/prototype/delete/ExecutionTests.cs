using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.delete") { }

    [Fact(DisplayName = "delete-entry")]
    public Task delete_entry()
        => ExecutionTestFromFile("delete-entry");

    [Fact(DisplayName = "returns-false-when-delete-is-noop")]
    public Task returns_false_when_delete_is_noop()
        => ExecutionTestFromFile("returns-false-when-delete-is-noop");

    [Fact(DisplayName = "delete")]
    public Task delete()
        => ExecutionTestFromFile("delete");

    [Fact(DisplayName = "returns-true-when-delete-operation-occurs")]
    public Task returns_true_when_delete_operation_occurs()
        => ExecutionTestFromFile("returns-true-when-delete-operation-occurs");

    [Fact(DisplayName = "delete-entry-normalizes-zero")]
    public Task delete_entry_normalizes_zero()
        => ExecutionTestFromFile("delete-entry-normalizes-zero");

    [Fact(DisplayName = "this-not-object-throw-undefined")]
    public Task this_not_object_throw_undefined()
        => ExecutionTestFromFile("this-not-object-throw-undefined");

    [Fact(DisplayName = "delete-entry-initial-iterable")]
    public Task delete_entry_initial_iterable()
        => ExecutionTestFromFile("delete-entry-initial-iterable");
}
