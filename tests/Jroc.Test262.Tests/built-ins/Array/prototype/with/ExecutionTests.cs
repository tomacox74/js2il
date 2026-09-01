using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.with;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.with") { }

    [Fact(DisplayName = "frozen-this-value")]
    public Task frozen_this_value()
        => ExecutionTestFromFile("frozen-this-value");

    [Fact(DisplayName = "index-throw-completion")]
    public Task index_throw_completion()
        => ExecutionTestFromFile("index-throw-completion");

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
}
