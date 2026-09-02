using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.union;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.union") { }

    [Fact(DisplayName = "add-not-called.js")]
    public Task add_not_called() => ExecutionTestFromFile("add-not-called");

    [Fact(DisplayName = "appends-new-values.js")]
    public Task appends_new_values() => ExecutionTestFromFile("appends-new-values");

    [Fact(DisplayName = "builtins.js")]
    public Task builtins() => ExecutionTestFromFile("builtins");

    [Fact(DisplayName = "called-with-object.js")]
    public Task called_with_object() => ExecutionTestFromFile("called-with-object");

    [Fact(DisplayName = "combines-Map.js")]
    public Task combines_Map() => ExecutionTestFromFile("combines-Map");

    [Fact(DisplayName = "combines-empty-sets.js")]
    public Task combines_empty_sets() => ExecutionTestFromFile("combines-empty-sets");

    [Fact(DisplayName = "combines-itself.js")]
    public Task combines_itself() => ExecutionTestFromFile("combines-itself");

    [Fact(DisplayName = "combines-same-sets.js")]
    public Task combines_same_sets() => ExecutionTestFromFile("combines-same-sets");

    [Fact(DisplayName = "combines-sets.js")]
    public Task combines_sets() => ExecutionTestFromFile("combines-sets");

    [Fact(DisplayName = "converts-negative-zero.js")]
    public Task converts_negative_zero() => ExecutionTestFromFile("converts-negative-zero");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "receiver-not-set.js")]
    public Task receiver_not_set() => ExecutionTestFromFile("receiver-not-set");

    [Fact(DisplayName = "require-internal-slot.js")]
    public Task require_internal_slot() => ExecutionTestFromFile("require-internal-slot");

    [Fact(DisplayName = "result-order.js")]
    public Task result_order() => ExecutionTestFromFile("result-order");

    [Fact(DisplayName = "set-like-array.js")]
    public Task set_like_array() => ExecutionTestFromFile("set-like-array");

    [Fact(DisplayName = "set-like-class-mutation.js")]
    public Task set_like_class_mutation() => ExecutionTestFromFile("set-like-class-mutation");

    [Fact(DisplayName = "set-like-class-order.js")]
    public Task set_like_class_order() => ExecutionTestFromFile("set-like-class-order");

    [Fact(DisplayName = "subclass-symbol-species.js")]
    public Task subclass_symbol_species() => ExecutionTestFromFile("subclass-symbol-species");

    [Fact(DisplayName = "subclass.js")]
    public Task subclass() => ExecutionTestFromFile("subclass");

    [Fact(DisplayName = "union.js")]
    public Task union() => ExecutionTestFromFile("union");

}
