using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.delete;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.delete") { }

    [Fact(DisplayName = "returns-false")]
    public Task returns_false()
        => ExecutionTestFromFile("returns-false");
    [Fact(DisplayName = "context-is-not-map-object")]
    public Task context_is_not_map_object()
        => ExecutionTestFromFile("context-is-not-map-object");

    [Fact(DisplayName = "context-is-not-object")]
    public Task context_is_not_object()
        => ExecutionTestFromFile("context-is-not-object");

    [Fact(DisplayName = "context-is-set-object-throws")]
    public Task context_is_set_object_throws()
        => ExecutionTestFromFile("context-is-set-object-throws");

    [Fact(DisplayName = "context-is-weakmap-object-throws")]
    public Task context_is_weakmap_object_throws()
        => ExecutionTestFromFile("context-is-weakmap-object-throws");

    [Fact(DisplayName = "does-not-break-iterators")]
    public Task does_not_break_iterators()
        => ExecutionTestFromFile("does-not-break-iterators");

    [Fact(DisplayName = "returns-true-for-deleted-entry")]
    public Task returns_true_for_deleted_entry()
        => ExecutionTestFromFile("returns-true-for-deleted-entry");

}
