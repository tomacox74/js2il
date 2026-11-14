using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.undefined;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.undefined") { }

    [Fact(DisplayName = "15.1.1.3-0.js")]
    public Task Port400_15_1_1_3_0()
        => ExecutionTestFromFile("15.1.1.3-0");

    [Fact(DisplayName = "15.1.1.3-1.js")]
    public Task Port400_15_1_1_3_1()
        => ExecutionTestFromFile("15.1.1.3-1");

    [Fact(DisplayName = "15.1.1.3-2.js")]
    public Task Port400_15_1_1_3_2()
        => ExecutionTestFromFile("15.1.1.3-2");

    [Fact(DisplayName = "15.1.1.3-3.js")]
    public Task Port400_15_1_1_3_3()
        => ExecutionTestFromFile("15.1.1.3-3");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port400_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.1.1.3_A3_T2.js")]
    public Task Port400_S15_1_1_3_A3_T2()
        => ExecutionTestFromFile("S15.1.1.3_A3_T2");

    [Fact(DisplayName = "S15.1.1.3_A4.js")]
    public Task Port400_S15_1_1_3_A4()
        => ExecutionTestFromFile("S15.1.1.3_A4");
}
