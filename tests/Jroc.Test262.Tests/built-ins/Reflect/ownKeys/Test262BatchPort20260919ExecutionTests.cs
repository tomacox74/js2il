using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.ownKeys;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.ownKeys") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-after-define-property")]
    public Task order_after_define_property()
        => ExecutionTestFromFile("order-after-define-property");

    [Fact(DisplayName = "ownKeys")]
    public Task ownKeys()
        => ExecutionTestFromFile("ownKeys");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "return-array-with-own-keys-only")]
    public Task return_array_with_own_keys_only()
        => ExecutionTestFromFile("return-array-with-own-keys-only");

    [Fact(DisplayName = "return-non-enumerable-keys")]
    public Task return_non_enumerable_keys()
        => ExecutionTestFromFile("return-non-enumerable-keys");

    [Fact(DisplayName = "return-on-corresponding-order-large-index")]
    public Task return_on_corresponding_order_large_index()
        => ExecutionTestFromFile("return-on-corresponding-order-large-index");

    [Fact(DisplayName = "return-on-corresponding-order")]
    public Task return_on_corresponding_order()
        => ExecutionTestFromFile("return-on-corresponding-order");

    [Fact(DisplayName = "target-is-not-object-throws")]
    public Task target_is_not_object_throws()
        => ExecutionTestFromFile("target-is-not-object-throws");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
