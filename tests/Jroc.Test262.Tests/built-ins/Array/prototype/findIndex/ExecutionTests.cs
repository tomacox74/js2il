using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.findIndex;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.findIndex") { }

    [Fact(DisplayName = "predicate-called-for-each-array-property")]
    public Task predicate_called_for_each_array_property()
        => ExecutionTestFromFile("predicate-called-for-each-array-property");

    [Fact(DisplayName = "return-index-predicate-result-is-true")]
    public Task return_index_predicate_result_is_true()
        => ExecutionTestFromFile("return-index-predicate-result-is-true");

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "array-altered-during-loop")]
    public Task array_altered_during_loop()
        => ExecutionTestFromFile("array-altered-during-loop");

    [Fact(DisplayName = "return-abrupt-from-property")]
    public Task return_abrupt_from_property()
        => ExecutionTestFromFile("return-abrupt-from-property");
}
