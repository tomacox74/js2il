namespace Jroc.Test262.Tests.built_ins.Object.values;

public partial class ExecutionTests
{
    [Fact(DisplayName = "function-property-descriptor.js")]
    public Task function_property_descriptor() => ExecutionTestFromFile("function-property-descriptor");

    [Fact(DisplayName = "getter-adding-key.js")]
    public Task getter_adding_key() => ExecutionTestFromFile("getter-adding-key");

    [Fact(DisplayName = "getter-making-future-key-nonenumerable.js")]
    public Task getter_making_future_key_nonenumerable() => ExecutionTestFromFile("getter-making-future-key-nonenumerable");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-after-define-property.js")]
    public Task order_after_define_property() => ExecutionTestFromFile("order-after-define-property");

    [Fact(DisplayName = "primitive-symbols.js")]
    public Task primitive_symbols() => ExecutionTestFromFile("primitive-symbols");

    [Fact(DisplayName = "return-order.js")]
    public Task return_order() => ExecutionTestFromFile("return-order");

    [Fact(DisplayName = "symbols-omitted.js")]
    public Task symbols_omitted() => ExecutionTestFromFile("symbols-omitted");

    [Fact(DisplayName = "tamper-with-global-object.js")]
    public Task tamper_with_global_object() => ExecutionTestFromFile("tamper-with-global-object");

    [Fact(DisplayName = "tamper-with-object-keys.js")]
    public Task tamper_with_object_keys() => ExecutionTestFromFile("tamper-with-object-keys");
}
