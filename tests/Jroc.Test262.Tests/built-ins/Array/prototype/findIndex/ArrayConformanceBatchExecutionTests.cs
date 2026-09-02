using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.findIndex;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype.findIndex") { }

    [Fact(DisplayName = "callbackfn-resize-arraybuffer.js")]
    public Task callbackfn_resize_arraybuffer() => ExecutionTestFromFile("callbackfn-resize-arraybuffer");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "predicate-call-parameters.js")]
    public Task predicate_call_parameters() => ExecutionTestFromFile("predicate-call-parameters");

    [Fact(DisplayName = "predicate-call-this-non-strict.js")]
    public Task predicate_call_this_non_strict() => ExecutionTestFromFile("predicate-call-this-non-strict");

    [Fact(DisplayName = "predicate-call-this-strict.js")]
    public Task predicate_call_this_strict() => ExecutionTestFromFile("predicate-call-this-strict");

    [Fact(DisplayName = "predicate-is-not-callable-throws.js")]
    public Task predicate_is_not_callable_throws() => ExecutionTestFromFile("predicate-is-not-callable-throws");

    [Fact(DisplayName = "predicate-not-called-on-empty-array.js")]
    public Task predicate_not_called_on_empty_array() => ExecutionTestFromFile("predicate-not-called-on-empty-array");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-abrupt-from-predicate-call.js")]
    public Task return_abrupt_from_predicate_call() => ExecutionTestFromFile("return-abrupt-from-predicate-call");

    [Fact(DisplayName = "return-abrupt-from-this-length-as-symbol.js")]
    public Task return_abrupt_from_this_length_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-length-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-this-length.js")]
    public Task return_abrupt_from_this_length() => ExecutionTestFromFile("return-abrupt-from-this-length");

    [Fact(DisplayName = "return-abrupt-from-this.js")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");

    [Fact(DisplayName = "return-negative-one-if-predicate-returns-false-value.js")]
    public Task return_negative_one_if_predicate_returns_false_value() => ExecutionTestFromFile("return-negative-one-if-predicate-returns-false-value");

}
