using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype") { }

    [Fact(DisplayName = "S15.5.4_A1")]
    public Task S15_5_4_A1()
        => ExecutionTestFromFile("S15.5.4_A1");

    [Fact(DisplayName = "S15.5.4_A2")]
    public Task S15_5_4_A2()
        => ExecutionTestFromFile("S15.5.4_A2");

    [Fact(DisplayName = "S15.5.4_A3")]
    public Task S15_5_4_A3()
        => ExecutionTestFromFile("S15.5.4_A3");
}
