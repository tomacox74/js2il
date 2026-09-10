using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.DisposableStack") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "instance-extensible")]
    public Task instance_extensible() => ExecutionTestFromFile("instance-extensible");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
}
