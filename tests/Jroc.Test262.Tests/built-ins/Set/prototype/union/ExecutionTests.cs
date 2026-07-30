using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.union;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.union") { }

    [Fact(DisplayName = "allows-set-like-object")]
    public Task allows_set_like_object()
        => ExecutionTestFromFile("allows-set-like-object");

    [Fact(DisplayName = "allows-set-like-class")]
    public Task allows_set_like_class()
        => ExecutionTestFromFile("allows-set-like-class");

    [Fact(DisplayName = "array-throws")]
    public Task array_throws()
        => ExecutionTestFromFile("array-throws");

    [Fact(DisplayName = "has-is-callable")]
    public Task has_is_callable()
        => ExecutionTestFromFile("has-is-callable");

    [Fact(DisplayName = "keys-is-callable")]
    public Task keys_is_callable()
        => ExecutionTestFromFile("keys-is-callable");

    [Fact(DisplayName = "size-is-a-number")]
    public Task size_is_a_number()
        => ExecutionTestFromFile("size-is-a-number");
}
