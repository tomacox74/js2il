using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toReversed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.toReversed") { }

    [Fact(DisplayName = "immutable")]
    public Task immutable()
        => ExecutionTestFromFile("immutable");

    [Fact(DisplayName = "zero-or-one-element")]
    public Task zero_or_one_element()
        => ExecutionTestFromFile("zero-or-one-element");

    [Fact(DisplayName = "holes-not-preserved")]
    public Task holes_not_preserved()
        => ExecutionTestFromFile("holes-not-preserved");


    [Fact(DisplayName = "length-increased-while-iterating")]
    public Task length_increased_while_iterating()
        => ExecutionTestFromFile("length-increased-while-iterating");

    [Fact(DisplayName = "length-decreased-while-iterating")]
    public Task length_decreased_while_iterating()
        => ExecutionTestFromFile("length-decreased-while-iterating");


}
