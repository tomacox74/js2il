using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.findLast;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.findLast") { }

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean() => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "maximum-index")]
    public Task maximum_index() => ExecutionTestFromFile("maximum-index");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "predicate-call-this-non-strict")]
    public Task predicate_call_this_non_strict() => ExecutionTestFromFile("predicate-call-this-non-strict");

    [Fact(DisplayName = "predicate-is-not-callable-throws")]
    public Task predicate_is_not_callable_throws() => ExecutionTestFromFile("predicate-is-not-callable-throws");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-abrupt-from-predicate-call")]
    public Task return_abrupt_from_predicate_call() => ExecutionTestFromFile("return-abrupt-from-predicate-call");

    [Fact(DisplayName = "return-abrupt-from-this")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");

    [Fact(DisplayName = "array-altered-during-loop")]
    public Task array_altered_during_loop()
        => ExecutionTestFromFile("array-altered-during-loop");

    [Fact(DisplayName = "predicate-call-parameters")]
    public Task predicate_call_parameters()
        => ExecutionTestFromFile("predicate-call-parameters");

    [Fact(DisplayName = "predicate-called-for-each-array-property")]
    public Task predicate_called_for_each_array_property()
        => ExecutionTestFromFile("predicate-called-for-each-array-property");

    [Fact(DisplayName = "predicate-not-called-on-empty-array")]
    public Task predicate_not_called_on_empty_array()
        => ExecutionTestFromFile("predicate-not-called-on-empty-array");

    [Fact(DisplayName = "return-found-value-predicate-result-is-true")]
    public Task return_found_value_predicate_result_is_true()
        => ExecutionTestFromFile("return-found-value-predicate-result-is-true");

    [Fact(DisplayName = "return-undefined-if-predicate-returns-false-value")]
    public Task return_undefined_if_predicate_returns_false_value()
        => ExecutionTestFromFile("return-undefined-if-predicate-returns-false-value");

}
