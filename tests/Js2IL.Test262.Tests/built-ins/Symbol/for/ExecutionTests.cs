using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Symbol.for_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.for_") { }

    [Fact(DisplayName = "create-value")]
    public Task create_value()
        => ExecutionTestFromFile("create-value");

    [Fact(DisplayName = "description")]
    public Task description()
        => ExecutionTestFromFile("description");

    [Fact(DisplayName = "retrieve-value")]
    public Task retrieve_value()
        => ExecutionTestFromFile("retrieve-value");
}
