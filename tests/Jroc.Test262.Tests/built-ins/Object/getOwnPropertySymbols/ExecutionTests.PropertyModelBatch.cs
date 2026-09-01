namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertySymbols;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-object-argument-invalid.js")]
    public Task non_object_argument_invalid() => ExecutionTestFromFile("non-object-argument-invalid");

    [Fact(DisplayName = "non-object-argument-valid.js")]
    public Task non_object_argument_valid() => ExecutionTestFromFile("non-object-argument-valid");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "object-contains-symbol-property-with-description.js")]
    public Task object_contains_symbol_property_with_description() => ExecutionTestFromFile("object-contains-symbol-property-with-description");

    [Fact(DisplayName = "object-contains-symbol-property-without-description.js")]
    public Task object_contains_symbol_property_without_description() => ExecutionTestFromFile("object-contains-symbol-property-without-description");

    [Fact(DisplayName = "order-after-define-property.js")]
    public Task order_after_define_property() => ExecutionTestFromFile("order-after-define-property");
}
