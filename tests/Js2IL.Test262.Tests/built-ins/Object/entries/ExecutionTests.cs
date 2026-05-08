using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.entries;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.entries") { }

    [Fact(DisplayName = "exception-during-enumeration")]
    public Task exception_during_enumeration()
        => ExecutionTestFromFile("exception-during-enumeration");

    [Fact(DisplayName = "exception-not-object-coercible")]
    public Task exception_not_object_coercible()
        => ExecutionTestFromFile("exception-not-object-coercible");

    [Fact(DisplayName = "function-length")]
    public Task function_length()
        => ExecutionTestFromFile("function-length");

    [Fact(DisplayName = "function-name")]
    public Task function_name()
        => ExecutionTestFromFile("function-name");

    [Fact(DisplayName = "function-property-descriptor")]
    public Task function_property_descriptor()
        => ExecutionTestFromFile("function-property-descriptor");

    [Fact(DisplayName = "getter-adding-key")]
    public Task getter_adding_key()
        => ExecutionTestFromFile("getter-adding-key");

    [Fact(DisplayName = "getter-making-future-key-nonenumerable")]
    public Task getter_making_future_key_nonenumerable()
        => ExecutionTestFromFile("getter-making-future-key-nonenumerable");

    [Fact(DisplayName = "getter-removing-future-key")]
    public Task getter_removing_future_key()
        => ExecutionTestFromFile("getter-removing-future-key");

    [Fact(DisplayName = "inherited-properties-omitted")]
    public Task inherited_properties_omitted()
        => ExecutionTestFromFile("inherited-properties-omitted");

    [Fact(DisplayName = "order-after-define-property-with-function")]
    public Task order_after_define_property_with_function()
        => ExecutionTestFromFile("order-after-define-property-with-function");

    [Fact(DisplayName = "order-after-define-property")]
    public Task order_after_define_property()
        => ExecutionTestFromFile("order-after-define-property");

    [Fact(DisplayName = "primitive-booleans")]
    public Task primitive_booleans()
        => ExecutionTestFromFile("primitive-booleans");

    [Fact(DisplayName = "primitive-numbers")]
    public Task primitive_numbers()
        => ExecutionTestFromFile("primitive-numbers");

    [Fact(DisplayName = "primitive-strings")]
    public Task primitive_strings()
        => ExecutionTestFromFile("primitive-strings");

    [Fact(DisplayName = "primitive-symbols")]
    public Task primitive_symbols()
        => ExecutionTestFromFile("primitive-symbols");

    [Fact(DisplayName = "return-order")]
    public Task return_order()
        => ExecutionTestFromFile("return-order");

    [Fact(DisplayName = "symbols-omitted")]
    public Task symbols_omitted()
        => ExecutionTestFromFile("symbols-omitted");

    [Fact(DisplayName = "tamper-with-global-object")]
    public Task tamper_with_global_object()
        => ExecutionTestFromFile("tamper-with-global-object");

    [Fact(DisplayName = "tamper-with-object-keys")]
    public Task tamper_with_object_keys()
        => ExecutionTestFromFile("tamper-with-object-keys");
}
