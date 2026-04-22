using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.GeneratorFunction;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorFunction") { }

    [Fact(DisplayName = "GeneratorFunction_length")]
    public Task GeneratorFunction_length()
        => ExecutionTest("GeneratorFunction_length");
}
