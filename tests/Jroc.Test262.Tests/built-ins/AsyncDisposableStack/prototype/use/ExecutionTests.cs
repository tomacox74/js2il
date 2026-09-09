using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack.prototype.use;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack.prototype.use") { }

    [Fact(DisplayName = "Symbol.asyncDispose-getter.js")]
    public Task Symbol_asyncDispose_getter() => ExecutionTestFromFile("Symbol.asyncDispose-getter");

    [Fact(DisplayName = "Symbol.dispose-getter.js")]
    public Task Symbol_dispose_getter() => ExecutionTestFromFile("Symbol.dispose-getter");

    [Fact(DisplayName = "adds-async-disposable-value.js")]
    public Task adds_async_disposable_value() => ExecutionTestFromFile("adds-async-disposable-value");

    [Fact(DisplayName = "adds-sync-disposable-value.js")]
    public Task adds_sync_disposable_value() => ExecutionTestFromFile("adds-sync-disposable-value");

    [Fact(DisplayName = "allows-null-value.js")]
    public Task allows_null_value() => ExecutionTestFromFile("allows-null-value");

    [Fact(DisplayName = "allows-undefined-value.js")]
    public Task allows_undefined_value() => ExecutionTestFromFile("allows-undefined-value");

    [Fact(DisplayName = "gets-value-Symbol.asyncDispose-property-once.js")]
    public Task gets_value_Symbol_asyncDispose_property_once() => ExecutionTestFromFile("gets-value-Symbol.asyncDispose-property-once");

    [Fact(DisplayName = "gets-value-Symbol.dispose-property-after-trying-Symbol.asyncDispose.js")]
    public Task gets_value_Symbol_dispose_property_after_trying_Symbol_asyncDispose() => ExecutionTestFromFile("gets-value-Symbol.dispose-property-after-trying-Symbol.asyncDispose");

    [Fact(DisplayName = "gets-value-Symbol.dispose-property-once.js")]
    public Task gets_value_Symbol_dispose_property_once() => ExecutionTestFromFile("gets-value-Symbol.dispose-property-once");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "puts-value-on-top-of-stack.js")]
    public Task puts_value_on_top_of_stack() => ExecutionTestFromFile("puts-value-on-top-of-stack");

    [Fact(DisplayName = "returns-value.js")]
    public Task returns_value() => ExecutionTestFromFile("returns-value");

    [Fact(DisplayName = "this-not-object-throws.js")]
    public Task this_not_object_throws() => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "throws-if-disposed.js")]
    public Task throws_if_disposed() => ExecutionTestFromFile("throws-if-disposed");

    [Fact(DisplayName = "throws-if-value-Symbol.asyncDispose-property-is-null-or-undefined.js")]
    public Task throws_if_value_Symbol_asyncDispose_property_is_null_or_undefined() => ExecutionTestFromFile("throws-if-value-Symbol.asyncDispose-property-is-null-or-undefined");

    [Fact(DisplayName = "throws-if-value-Symbol.asyncDispose-property-not-callable.js")]
    public Task throws_if_value_Symbol_asyncDispose_property_not_callable() => ExecutionTestFromFile("throws-if-value-Symbol.asyncDispose-property-not-callable");

    [Fact(DisplayName = "throws-if-value-Symbol.dispose-property-is-null-or-undefined.js")]
    public Task throws_if_value_Symbol_dispose_property_is_null_or_undefined() => ExecutionTestFromFile("throws-if-value-Symbol.dispose-property-is-null-or-undefined");

    [Fact(DisplayName = "throws-if-value-Symbol.dispose-property-not-callable.js")]
    public Task throws_if_value_Symbol_dispose_property_not_callable() => ExecutionTestFromFile("throws-if-value-Symbol.dispose-property-not-callable");

    [Fact(DisplayName = "throws-if-value-missing-Symbol.asyncDispose-and-Symbol.dispose.js")]
    public Task throws_if_value_missing_Symbol_asyncDispose_and_Symbol_dispose() => ExecutionTestFromFile("throws-if-value-missing-Symbol.asyncDispose-and-Symbol.dispose");

    [Fact(DisplayName = "throws-if-value-not-object.js")]
    public Task throws_if_value_not_object() => ExecutionTestFromFile("throws-if-value-not-object");
}
