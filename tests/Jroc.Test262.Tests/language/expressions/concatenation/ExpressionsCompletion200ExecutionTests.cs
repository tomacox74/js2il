namespace Jroc.Test262.Tests.language.expressions.concatenation;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.concatenation") { }

    [Fact(DisplayName = "S9.8_A1_T2.js", Skip = "eval is not supported.")]
    public Task S9_8_A1_T2()
        => ExecutionTest("S9.8_A1_T2");

    [Fact(DisplayName = "S9.8_A2_T2.js")]
    public Task S9_8_A2_T2()
        => ExecutionTest("S9.8_A2_T2");

    [Fact(DisplayName = "S9.8_A3_T2.js")]
    public Task S9_8_A3_T2()
        => ExecutionTest("S9.8_A3_T2");

    [Fact(DisplayName = "S9.8_A4_T2.js")]
    public Task S9_8_A4_T2()
        => ExecutionTest("S9.8_A4_T2");

    [Fact(DisplayName = "S9.8_A5_T2.js")]
    public Task S9_8_A5_T2()
        => ExecutionTest("S9.8_A5_T2");
}
