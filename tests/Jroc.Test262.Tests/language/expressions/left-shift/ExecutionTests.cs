using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.left_shift;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.left_shift") { }

    [Fact(DisplayName = "S11.7.1_A5.1_T1")]
    public Task S11_7_1_A5_1_T1()
        => ExecutionTest("S11.7.1_A5.1_T1");

    [Fact(DisplayName = "S9.5_A1_T1")]
    public Task S9_5_A1_T1()
        => ExecutionTest("S9.5_A1_T1");

    [Fact(DisplayName = "S9.5_A2.1_T1")]
    public Task S9_5_A2_1_T1()
        => ExecutionTest("S9.5_A2.1_T1");

    [Fact(DisplayName = "S9.5_A2.2_T1")]
    public Task S9_5_A2_2_T1()
        => ExecutionTest("S9.5_A2.2_T1");

    [Fact(DisplayName = "S9.5_A2.3_T1")]
    public Task S9_5_A2_3_T1()
        => ExecutionTest("S9.5_A2.3_T1");
}
