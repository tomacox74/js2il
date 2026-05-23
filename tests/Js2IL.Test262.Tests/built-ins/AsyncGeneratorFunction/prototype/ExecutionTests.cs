using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.AsyncGeneratorFunction.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction.prototype") { }

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");
}
