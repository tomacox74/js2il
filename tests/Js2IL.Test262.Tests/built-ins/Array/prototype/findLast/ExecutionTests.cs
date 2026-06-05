using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.findLast;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.findLast") { }

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
