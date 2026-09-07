using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.numeric;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.numeric") { }

    [Fact(DisplayName = "S7.8.3_A4.2_T1.js")]
    public Task S7_8_3_A4_2_T1()
        => ExecutionTest("S7.8.3_A4.2_T1");

    [Fact(DisplayName = "S7.8.3_A4.2_T2.js")]
    public Task S7_8_3_A4_2_T2()
        => ExecutionTest("S7.8.3_A4.2_T2");

    [Fact(DisplayName = "S7.8.3_A4.2_T3.js")]
    public Task S7_8_3_A4_2_T3()
        => ExecutionTest("S7.8.3_A4.2_T3");

    [Fact(DisplayName = "S7.8.3_A4.2_T4.js")]
    public Task S7_8_3_A4_2_T4()
        => ExecutionTest("S7.8.3_A4.2_T4");

    [Fact(DisplayName = "S7.8.3_A4.2_T5.js")]
    public Task S7_8_3_A4_2_T5()
        => ExecutionTest("S7.8.3_A4.2_T5");

    [Fact(DisplayName = "S7.8.3_A4.2_T6.js")]
    public Task S7_8_3_A4_2_T6()
        => ExecutionTest("S7.8.3_A4.2_T6");

    [Fact(DisplayName = "S7.8.3_A4.2_T7.js")]
    public Task S7_8_3_A4_2_T7()
        => ExecutionTest("S7.8.3_A4.2_T7");

    [Fact(DisplayName = "S7.8.3_A4.2_T8.js")]
    public Task S7_8_3_A4_2_T8()
        => ExecutionTest("S7.8.3_A4.2_T8");

    [Fact(DisplayName = "S7.8.3_A5.1_T1.js")]
    public Task S7_8_3_A5_1_T1()
        => ExecutionTest("S7.8.3_A5.1_T1");

    [Fact(DisplayName = "S7.8.3_A5.1_T2.js")]
    public Task S7_8_3_A5_1_T2()
        => ExecutionTest("S7.8.3_A5.1_T2");

    [Fact(DisplayName = "S7.8.3_A5.1_T3.js")]
    public Task S7_8_3_A5_1_T3()
        => ExecutionTest("S7.8.3_A5.1_T3");

    [Fact(DisplayName = "S7.8.3_A5.1_T4.js")]
    public Task S7_8_3_A5_1_T4()
        => ExecutionTest("S7.8.3_A5.1_T4");

    [Fact(DisplayName = "S7.8.3_A5.1_T5.js")]
    public Task S7_8_3_A5_1_T5()
        => ExecutionTest("S7.8.3_A5.1_T5");

    [Fact(DisplayName = "S7.8.3_A5.1_T6.js")]
    public Task S7_8_3_A5_1_T6()
        => ExecutionTest("S7.8.3_A5.1_T6");

    [Fact(DisplayName = "S7.8.3_A5.1_T7.js")]
    public Task S7_8_3_A5_1_T7()
        => ExecutionTest("S7.8.3_A5.1_T7");

    [Fact(DisplayName = "S7.8.3_A5.1_T8.js")]
    public Task S7_8_3_A5_1_T8()
        => ExecutionTest("S7.8.3_A5.1_T8");

}
