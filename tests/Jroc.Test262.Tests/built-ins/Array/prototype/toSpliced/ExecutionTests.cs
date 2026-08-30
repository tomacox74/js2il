using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.toSpliced;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.toSpliced") { }

    [Fact(DisplayName = "this-value-boolean")]
    public Task this_value_boolean()
        => ExecutionTestFromFile("this-value-boolean");

    [Fact(DisplayName = "length-exceeding-array-length-limit")]
    public Task length_exceeding_array_length_limit()
        => ExecutionTestFromFile("length-exceeding-array-length-limit");
}
