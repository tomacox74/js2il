using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.add;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.add") { }

    [Fact(DisplayName = "preserves-insertion-order")]
    public Task preserves_insertion_order()
        => ExecutionTestFromFile("preserves-insertion-order");

    [Fact(DisplayName = "returns-this-when-ignoring-duplicate")]
    public Task returns_this_when_ignoring_duplicate()
        => ExecutionTestFromFile("returns-this-when-ignoring-duplicate");

    [Fact(DisplayName = "returns-this")]
    public Task returns_this()
        => ExecutionTestFromFile("returns-this");

    [Fact(DisplayName = "add")]
    public Task add()
        => ExecutionTestFromFile("add");

    [Fact(DisplayName = "will-not-add-duplicate-entry")]
    public Task will_not_add_duplicate_entry()
        => ExecutionTestFromFile("will-not-add-duplicate-entry");

    [Fact(DisplayName = "will-not-add-duplicate-entry-normalizes-zero")]
    public Task will_not_add_duplicate_entry_normalizes_zero()
        => ExecutionTestFromFile("will-not-add-duplicate-entry-normalizes-zero");

    [Fact(DisplayName = "this-not-object-throw-undefined")]
    public Task this_not_object_throw_undefined()
        => ExecutionTestFromFile("this-not-object-throw-undefined");
}
