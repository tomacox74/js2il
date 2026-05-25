using Js2IL.Test262.Tests.built_ins;


namespace Js2IL.Test262.Tests.built_ins.Array.prototype.find;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.find") { }

    [Fact(DisplayName = "predicate-called-for-each-array-property")]
    public Task predicate_called_for_each_array_property()
        => ExecutionTestFromFile("predicate-called-for-each-array-property");

    [Fact(DisplayName = "return-found-value-predicate-result-is-true")]
    public Task return_found_value_predicate_result_is_true()
        => ExecutionTestFromFile("return-found-value-predicate-result-is-true");
}
