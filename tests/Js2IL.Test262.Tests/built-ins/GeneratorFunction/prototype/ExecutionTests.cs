using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.GeneratorFunction.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorFunction.prototype") { }

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");
}
