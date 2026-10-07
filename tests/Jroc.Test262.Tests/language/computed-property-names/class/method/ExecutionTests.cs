using Xunit;

namespace Jroc.Test262.Tests.language.computed_property_names.@class.method;

public sealed class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.computed-property-names.class.method")
    {
    }

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-can-be-generator.js")]
    public Task test_constructor_can_be_generator() => ExecutionTest("constructor-can-be-generator");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-can-be-getter.js")]
    public Task test_constructor_can_be_getter() => ExecutionTest("constructor-can-be-getter");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-can-be-setter.js")]
    public Task test_constructor_can_be_setter() => ExecutionTest("constructor-can-be-setter");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-duplicate-1.js")]
    public Task test_constructor_duplicate_1() => ExecutionTest("constructor-duplicate-1");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-duplicate-2.js")]
    public Task test_constructor_duplicate_2() => ExecutionTest("constructor-duplicate-2");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor-duplicate-3.js")]
    public Task test_constructor_duplicate_3() => ExecutionTest("constructor-duplicate-3");

    [Fact(DisplayName = "language/computed-property-names/class/method/constructor.js")]
    public Task test_constructor() => ExecutionTest("constructor");

    [Fact(DisplayName = "language/computed-property-names/class/method/generator.js")]
    public Task test_generator() => ExecutionTest("generator");

    [Fact(DisplayName = "language/computed-property-names/class/method/number.js")]
    public Task test_number() => ExecutionTest("number");

    [Fact(DisplayName = "language/computed-property-names/class/method/string.js")]
    public Task test_string() => ExecutionTest("string");

    [Fact(DisplayName = "language/computed-property-names/class/method/symbol.js")]
    public Task test_symbol() => ExecutionTest("symbol");
}
