using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NaN;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.NaN") { }

    [Fact(DisplayName = "S15.1.1.1_A2_T2")]
    public Task S15_1_1_1_A2_T2()
        => ExecutionTestFromFile("S15.1.1.1_A2_T2");

    [Fact(DisplayName = "S15.1.1.1_A4")]
    public Task S15_1_1_1_A4()
        => ExecutionTestFromFile("S15.1.1.1_A4");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
