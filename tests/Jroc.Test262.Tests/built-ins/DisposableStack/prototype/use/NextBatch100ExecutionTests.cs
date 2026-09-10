using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack.prototype.use;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack.prototype.use") { }

    [Fact(DisplayName = "Symbol.dispose-getter")]
    public Task Symbol_dispose_getter() => ExecutionTestFromFile("Symbol.dispose-getter");

    [Fact(DisplayName = "adds-value")]
    public Task adds_value() => ExecutionTestFromFile("adds-value");

    [Fact(DisplayName = "allows-null-value")]
    public Task allows_null_value() => ExecutionTestFromFile("allows-null-value");

    [Fact(DisplayName = "allows-undefined-value")]
    public Task allows_undefined_value() => ExecutionTestFromFile("allows-undefined-value");

    [Fact(DisplayName = "gets-value-Symbol.dispose-property-once")]
    public Task gets_value_Symbol_dispose_property_once() => ExecutionTestFromFile("gets-value-Symbol.dispose-property-once");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "puts-value-on-top-of-stack")]
    public Task puts_value_on_top_of_stack() => ExecutionTestFromFile("puts-value-on-top-of-stack");

    [Fact(DisplayName = "returns-value")]
    public Task returns_value() => ExecutionTestFromFile("returns-value");

    [Fact(DisplayName = "this-does-not-have-internal-disposablestate-throws")]
    public Task this_does_not_have_internal_disposablestate_throws() => ExecutionTestFromFile("this-does-not-have-internal-disposablestate-throws");

    [Fact(DisplayName = "this-not-object-throws")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");

    [Fact(DisplayName = "throws-if-value-Symbol.dispose-property-is-null")]
    public Task throws_if_value_Symbol_dispose_property_is_null() => ExecutionTestFromFile("throws-if-value-Symbol.dispose-property-is-null");

    [Fact(DisplayName = "throws-if-value-Symbol.dispose-property-is-undefined")]
    public Task throws_if_value_Symbol_dispose_property_is_undefined() => ExecutionTestFromFile("throws-if-value-Symbol.dispose-property-is-undefined");

    [Fact(DisplayName = "throws-if-value-Symbol.dispose-property-not-callable")]
    public Task throws_if_value_Symbol_dispose_property_not_callable() => ExecutionTestFromFile("throws-if-value-Symbol.dispose-property-not-callable");

    [Fact(DisplayName = "throws-if-value-missing-Symbol.dispose")]
    public Task throws_if_value_missing_Symbol_dispose() => ExecutionTestFromFile("throws-if-value-missing-Symbol.dispose");

    [Fact(DisplayName = "throws-if-value-not-object")]
    public Task throws_if_value_not_object() => ExecutionTestFromFile("throws-if-value-not-object");
}
