using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Boolean.prototype") { }

    [Fact(DisplayName = "S15.6.3.1_A2")]
    public Task S15_6_3_1_A2()
        => ExecutionTestFromFile("S15.6.3.1_A2");

    [Fact(DisplayName = "S15.6.3.1_A3")]
    public Task S15_6_3_1_A3()
        => ExecutionTestFromFile("S15.6.3.1_A3");

    [Fact(DisplayName = "S15.6.3.1_A4")]
    public Task S15_6_3_1_A4()
        => ExecutionTestFromFile("S15.6.3.1_A4");

    [Fact(DisplayName = "S15.6.4_A2")]
    public Task S15_6_4_A2()
        => ExecutionTestFromFile("S15.6.4_A2");

}
