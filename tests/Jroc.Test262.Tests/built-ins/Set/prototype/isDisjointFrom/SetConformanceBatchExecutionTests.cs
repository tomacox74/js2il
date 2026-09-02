using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.isDisjointFrom;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.isDisjointFrom") { }

    [Fact(DisplayName = "allows-set-like-class.js")]
    public Task allows_set_like_class() => ExecutionTestFromFile("allows-set-like-class");

    [Fact(DisplayName = "array-throws.js")]
    public Task array_throws() => ExecutionTestFromFile("array-throws");

    [Fact(DisplayName = "builtins.js")]
    public Task builtins() => ExecutionTestFromFile("builtins");

    [Fact(DisplayName = "called-with-object.js")]
    public Task called_with_object() => ExecutionTestFromFile("called-with-object");

    [Fact(DisplayName = "compares-Map.js")]
    public Task compares_Map() => ExecutionTestFromFile("compares-Map");

    [Fact(DisplayName = "compares-empty-sets.js")]
    public Task compares_empty_sets() => ExecutionTestFromFile("compares-empty-sets");

    [Fact(DisplayName = "compares-itself.js")]
    public Task compares_itself() => ExecutionTestFromFile("compares-itself");

    [Fact(DisplayName = "compares-same-sets.js")]
    public Task compares_same_sets() => ExecutionTestFromFile("compares-same-sets");

    [Fact(DisplayName = "compares-sets.js")]
    public Task compares_sets() => ExecutionTestFromFile("compares-sets");

    [Fact(DisplayName = "converts-negative-zero.js")]
    public Task converts_negative_zero() => ExecutionTestFromFile("converts-negative-zero");

    [Fact(DisplayName = "has-is-callable.js")]
    public Task has_is_callable() => ExecutionTestFromFile("has-is-callable");

    [Fact(DisplayName = "isDisjointFrom.js")]
    public Task isDisjointFrom() => ExecutionTestFromFile("isDisjointFrom");

    [Fact(DisplayName = "keys-is-callable.js")]
    public Task keys_is_callable() => ExecutionTestFromFile("keys-is-callable");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "receiver-not-set.js")]
    public Task receiver_not_set() => ExecutionTestFromFile("receiver-not-set");

    [Fact(DisplayName = "require-internal-slot.js")]
    public Task require_internal_slot() => ExecutionTestFromFile("require-internal-slot");

    [Fact(DisplayName = "set-like-array.js")]
    public Task set_like_array() => ExecutionTestFromFile("set-like-array");

    [Fact(DisplayName = "set-like-class-mutation.js")]
    public Task set_like_class_mutation() => ExecutionTestFromFile("set-like-class-mutation");

    [Fact(DisplayName = "set-like-class-order.js")]
    public Task set_like_class_order() => ExecutionTestFromFile("set-like-class-order");

    [Fact(DisplayName = "set-like-iter-return.js")]
    public Task set_like_iter_return() => ExecutionTestFromFile("set-like-iter-return");

    [Fact(DisplayName = "size-is-a-number.js")]
    public Task size_is_a_number() => ExecutionTestFromFile("size-is-a-number");

}
