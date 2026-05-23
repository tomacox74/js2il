using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.AsyncGeneratorFunction;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction") { }

    [Fact(DisplayName = "AsyncGeneratorFunction_length")]
    public Task AsyncGeneratorFunction_length()
        => ExecutionTestFromFile("AsyncGeneratorFunction_length");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "has-instance", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task has_instance()
        => ExecutionTestFromFile("has-instance");
}
