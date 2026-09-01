namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertyDescriptors;

public partial class ExecutionTests
{
    [Fact(DisplayName = "exception-not-object-coercible.js")]
    public Task exception_not_object_coercible() => ExecutionTestFromFile("exception-not-object-coercible");

    [Fact(DisplayName = "function-length.js")]
    public Task function_length() => ExecutionTestFromFile("function-length");

    [Fact(DisplayName = "function-name.js")]
    public Task function_name() => ExecutionTestFromFile("function-name");

    [Fact(DisplayName = "function-property-descriptor.js")]
    public Task function_property_descriptor() => ExecutionTestFromFile("function-property-descriptor");

    [Fact(DisplayName = "inherited-properties-omitted.js")]
    public Task inherited_properties_omitted() => ExecutionTestFromFile("inherited-properties-omitted");

    [Fact(DisplayName = "normal-object.js")]
    public Task normal_object() => ExecutionTestFromFile("normal-object");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "observable-operations.js")]
    public Task observable_operations() => ExecutionTestFromFile("observable-operations");

    [Fact(DisplayName = "order-after-define-property.js")]
    public Task order_after_define_property() => ExecutionTestFromFile("order-after-define-property");

    [Fact(DisplayName = "primitive-booleans.js")]
    public Task primitive_booleans() => ExecutionTestFromFile("primitive-booleans");

    [Fact(DisplayName = "primitive-numbers.js")]
    public Task primitive_numbers() => ExecutionTestFromFile("primitive-numbers");

    [Fact(DisplayName = "primitive-strings.js")]
    public Task primitive_strings() => ExecutionTestFromFile("primitive-strings");

    [Fact(DisplayName = "primitive-symbols.js")]
    public Task primitive_symbols() => ExecutionTestFromFile("primitive-symbols");

    [Fact(DisplayName = "proxy-no-ownkeys-returned-keys-order.js")]
    public Task proxy_no_ownkeys_returned_keys_order() => ExecutionTestFromFile("proxy-no-ownkeys-returned-keys-order");

    [Fact(DisplayName = "proxy-undefined-descriptor.js")]
    public Task proxy_undefined_descriptor() => ExecutionTestFromFile("proxy-undefined-descriptor");

    [Fact(DisplayName = "symbols-included.js")]
    public Task symbols_included() => ExecutionTestFromFile("symbols-included");

    [Fact(DisplayName = "tamper-with-global-object.js")]
    public Task tamper_with_global_object() => ExecutionTestFromFile("tamper-with-global-object");

    [Fact(DisplayName = "tamper-with-object-keys.js")]
    public Task tamper_with_object_keys() => ExecutionTestFromFile("tamper-with-object-keys");
}
