using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.AsyncGeneratorFunction;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncGeneratorFunction") { }

    [Fact(DisplayName = "AsyncGeneratorFunction_length")]
    public Task AsyncGeneratorFunction_length()
        => ExecutionTest("AsyncGeneratorFunction_length");
}
