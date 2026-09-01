using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toSorted;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.toSorted") { }

    [Fact(DisplayName = "immutable")]
    public Task immutable()
        => ExecutionTestFromFile("immutable");

    [Fact(DisplayName = "comparefn-called-after-get-elements")]
    public Task comparefn_called_after_get_elements()
        => ExecutionTestFromFile("comparefn-called-after-get-elements");

    [Fact(DisplayName = "comparefn-not-a-function")]
    public Task comparefn_not_a_function()
        => ExecutionTestFromFile("comparefn-not-a-function");

    [Fact(DisplayName = "comparefn-stop-after-error")]
    public Task comparefn_stop_after_error()
        => ExecutionTestFromFile("comparefn-stop-after-error");

    [Fact(DisplayName = "frozen-this-value")]
    public Task frozen_this_value()
        => ExecutionTestFromFile("frozen-this-value");

    [Fact(DisplayName = "length-casted-to-zero")]
    public Task length_casted_to_zero()
        => ExecutionTestFromFile("length-casted-to-zero");

    [Fact(DisplayName = "length-decreased-while-iterating")]
    public Task length_decreased_while_iterating()
        => ExecutionTestFromFile("length-decreased-while-iterating");

    [Fact(DisplayName = "length-exceeding-array-length-limit")]
    public Task length_exceeding_array_length_limit()
        => ExecutionTestFromFile("length-exceeding-array-length-limit");

    [Fact(DisplayName = "length-increased-while-iterating")]
    public Task length_increased_while_iterating()
        => ExecutionTestFromFile("length-increased-while-iterating");

    [Fact(DisplayName = "length-tolength")]
    public Task length_tolength()
        => ExecutionTestFromFile("length-tolength");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
