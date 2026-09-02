using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findLast.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.findLast.BigInt") { }

    [Fact(DisplayName = "get-length-ignores-length-prop.js")]
    public Task get_length_ignores_length_prop() => ExecutionTestFromFile("get-length-ignores-length-prop");

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

    [Fact(DisplayName = "return-abrupt-from-predicate-call.js")]
    public Task return_abrupt_from_predicate_call() => ExecutionTestFromFile("return-abrupt-from-predicate-call");

    [Fact(DisplayName = "return-found-value-predicate-result-is-true.js")]
    public Task return_found_value_predicate_result_is_true() => ExecutionTestFromFile("return-found-value-predicate-result-is-true");

    [Fact(DisplayName = "return-undefined-if-predicate-returns-false-value.js")]
    public Task return_undefined_if_predicate_returns_false_value() => ExecutionTestFromFile("return-undefined-if-predicate-returns-false-value");

}
