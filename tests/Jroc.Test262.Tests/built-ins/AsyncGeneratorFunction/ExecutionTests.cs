using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorFunction;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction") { }

    [Fact(DisplayName = "AsyncGeneratorFunction_length")]
    public Task AsyncGeneratorFunction_length()
        => ExecutionTestFromFile("AsyncGeneratorFunction_length");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "has-instance")]
    public Task has_instance()
        => ExecutionTestFromFile("has-instance");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "instance-length")]
    public Task instance_length()
        => ExecutionTestFromFile("instance-length");

    [Fact(DisplayName = "instance-prototype")]
    public Task instance_prototype()
        => ExecutionTestFromFile("instance-prototype");
}
