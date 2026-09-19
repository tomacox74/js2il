using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Function") { }

    [Fact(DisplayName = "S15.3.2.1_A1_T11")]
    public Task S15_3_2_1_A1_T11()
        => ExecutionTestFromFile("S15.3.2.1_A1_T11");

    [Fact(DisplayName = "S15.3.2.1_A1_T12")]
    public Task S15_3_2_1_A1_T12()
        => ExecutionTestFromFile("S15.3.2.1_A1_T12");

    [Fact(DisplayName = "S15.3.2.1_A1_T4")]
    public Task S15_3_2_1_A1_T4()
        => ExecutionTestFromFile("S15.3.2.1_A1_T4");

}
