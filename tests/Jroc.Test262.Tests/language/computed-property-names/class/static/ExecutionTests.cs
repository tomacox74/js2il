using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@class.@static;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.class.static")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/class/static/generator-constructor.js")]
    public Task test_generator_constructor() => ExecutionTest("generator-constructor");

    [Fact(DisplayName = "language/computed-property-names/class/static/generator-prototype.js")]
    public Task test_generator_prototype() => ExecutionTest("generator-prototype");

    [Fact(DisplayName = "language/computed-property-names/class/static/getter-constructor.js")]
    public Task test_getter_constructor() => ExecutionTest("getter-constructor");

    [Fact(DisplayName = "language/computed-property-names/class/static/getter-prototype.js")]
    public Task test_getter_prototype() => ExecutionTest("getter-prototype");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-constructor.js")]
    public Task test_method_constructor() => ExecutionTest("method-constructor");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-number-order.js")]
    public Task test_method_number_order() => ExecutionTest("method-number-order");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-number.js")]
    public Task test_method_number() => ExecutionTest("method-number");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-prototype.js")]
    public Task test_method_prototype() => ExecutionTest("method-prototype");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-string-order.js")]
    public Task test_method_string_order() => ExecutionTest("method-string-order");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-string.js")]
    public Task test_method_string() => ExecutionTest("method-string");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-symbol-order.js")]
    public Task test_method_symbol_order() => ExecutionTest("method-symbol-order");

    [Fact(DisplayName = "language/computed-property-names/class/static/method-symbol.js")]
    public Task test_method_symbol() => ExecutionTest("method-symbol");

    [Fact(DisplayName = "language/computed-property-names/class/static/setter-constructor.js")]
    public Task test_setter_constructor() => ExecutionTest("setter-constructor");

    [Fact(DisplayName = "language/computed-property-names/class/static/setter-prototype.js")]
    public Task test_setter_prototype() => ExecutionTest("setter-prototype");
}
