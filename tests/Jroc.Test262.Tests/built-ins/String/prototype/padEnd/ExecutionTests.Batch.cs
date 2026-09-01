namespace Jroc.Test262.Tests.built_ins.String.prototype.padEnd;

public partial class ExecutionTests
{
    [Fact(DisplayName = "max-length-not-greater-than-string.js")]
    public Task max_length_not_greater_than_string() => ExecutionTestFromFile("max-length-not-greater-than-string");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "observable-operations.js")]
    public Task observable_operations() => ExecutionTestFromFile("observable-operations");
}
