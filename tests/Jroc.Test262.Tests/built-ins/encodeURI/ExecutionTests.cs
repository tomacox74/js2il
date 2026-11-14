using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.encodeURI;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.encodeURI") { }

    [Fact(DisplayName = "S15.1.3.3_A1.1_T1")]
    public Task S15_1_3_3_A1_1_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.1_T1");

    [Fact(DisplayName = "S15.1.3.3_A1.1_T2")]
    public Task S15_1_3_3_A1_1_T2()
        => ExecutionTestFromFile("S15.1.3.3_A1.1_T2");

    [Fact(DisplayName = "S15.1.3.3_A1.2_T1")]
    public Task S15_1_3_3_A1_2_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.2_T1");

    [Fact(DisplayName = "S15.1.3.3_A1.2_T2")]
    public Task S15_1_3_3_A1_2_T2()
        => ExecutionTestFromFile("S15.1.3.3_A1.2_T2");

    [Fact(DisplayName = "S15.1.3.3_A1.3_T1")]
    public Task S15_1_3_3_A1_3_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.3_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.1_T1")]
    public Task S15_1_3_3_A2_1_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.1_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.2_T1")]
    public Task S15_1_3_3_A2_2_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.2_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.3_T1")]
    public Task S15_1_3_3_A2_3_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.3_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.4_T1")]
    public Task S15_1_3_3_A2_4_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.4_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.4_T2")]
    public Task S15_1_3_3_A2_4_T2()
        => ExecutionTestFromFile("S15.1.3.3_A2.4_T2");

    [Fact(DisplayName = "name.js")]
    public Task Port400_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port400_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port400_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.1.3.3_A2.5_T1.js")]
    public Task Port400_S15_1_3_3_A2_5_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.5_T1");

    [Fact(DisplayName = "S15.1.3.3_A3.1_T1.js")]
    public Task Port400_S15_1_3_3_A3_1_T1()
        => ExecutionTestFromFile("S15.1.3.3_A3.1_T1");

    [Fact(DisplayName = "S15.1.3.3_A3.2_T1.js")]
    public Task Port400_S15_1_3_3_A3_2_T1()
        => ExecutionTestFromFile("S15.1.3.3_A3.2_T1");

    [Fact(DisplayName = "S15.1.3.3_A3.2_T2.js")]
    public Task Port400_S15_1_3_3_A3_2_T2()
        => ExecutionTestFromFile("S15.1.3.3_A3.2_T2");

    [Fact(DisplayName = "S15.1.3.3_A3.2_T3.js")]
    public Task Port400_S15_1_3_3_A3_2_T3()
        => ExecutionTestFromFile("S15.1.3.3_A3.2_T3");

    [Fact(DisplayName = "S15.1.3.3_A3.3_T1.js")]
    public Task Port400_S15_1_3_3_A3_3_T1()
        => ExecutionTestFromFile("S15.1.3.3_A3.3_T1");

    [Fact(DisplayName = "S15.1.3.3_A4_T1.js")]
    public Task Port400_S15_1_3_3_A4_T1()
        => ExecutionTestFromFile("S15.1.3.3_A4_T1");

    [Fact(DisplayName = "S15.1.3.3_A4_T2.js")]
    public Task Port400_S15_1_3_3_A4_T2()
        => ExecutionTestFromFile("S15.1.3.3_A4_T2");

    [Fact(DisplayName = "S15.1.3.3_A4_T3.js")]
    public Task Port400_S15_1_3_3_A4_T3()
        => ExecutionTestFromFile("S15.1.3.3_A4_T3");

    [Fact(DisplayName = "S15.1.3.3_A4_T4.js")]
    public Task Port400_S15_1_3_3_A4_T4()
        => ExecutionTestFromFile("S15.1.3.3_A4_T4");

    [Fact(DisplayName = "S15.1.3.3_A5.1.js")]
    public Task Port400_S15_1_3_3_A5_1()
        => ExecutionTestFromFile("S15.1.3.3_A5.1");

    [Fact(DisplayName = "S15.1.3.3_A5.2.js")]
    public Task Port400_S15_1_3_3_A5_2()
        => ExecutionTestFromFile("S15.1.3.3_A5.2");

    [Fact(DisplayName = "S15.1.3.3_A5.3.js")]
    public Task Port400_S15_1_3_3_A5_3()
        => ExecutionTestFromFile("S15.1.3.3_A5.3");

    [Fact(DisplayName = "S15.1.3.3_A5.4.js")]
    public Task Port400_S15_1_3_3_A5_4()
        => ExecutionTestFromFile("S15.1.3.3_A5.4");

    [Fact(DisplayName = "S15.1.3.3_A5.5.js")]
    public Task Port400_S15_1_3_3_A5_5()
        => ExecutionTestFromFile("S15.1.3.3_A5.5");

    [Fact(DisplayName = "S15.1.3.3_A5.6.js")]
    public Task Port400_S15_1_3_3_A5_6()
        => ExecutionTestFromFile("S15.1.3.3_A5.6");

    [Fact(DisplayName = "S15.1.3.3_A5.7.js")]
    public Task Port400_S15_1_3_3_A5_7()
        => ExecutionTestFromFile("S15.1.3.3_A5.7");

    [Fact(DisplayName = "S15.1.3.3_A6_T1.js")]
    public Task Port400_S15_1_3_3_A6_T1()
        => ExecutionTestFromFile("S15.1.3.3_A6_T1");
}
