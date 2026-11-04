namespace Jroc.Test262.Tests.language.statements.return_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.return_") { }

    [Fact(DisplayName = "12.9-1.js")]
    public Task test_12_9_1()
        => ExecutionTest("12.9-1");

    [Fact(DisplayName = "S12.9_A1_T1.js")]
    public Task test_S12_9_A1_T1()
        => CompilationFailureTest("S12.9_A1_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T10.js")]
    public Task test_S12_9_A1_T10()
        => CompilationFailureTest("S12.9_A1_T10", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T2.js")]
    public Task test_S12_9_A1_T2()
        => CompilationFailureTest("S12.9_A1_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T3.js")]
    public Task test_S12_9_A1_T3()
        => CompilationFailureTest("S12.9_A1_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T4.js")]
    public Task test_S12_9_A1_T4()
        => CompilationFailureTest("S12.9_A1_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T5.js")]
    public Task test_S12_9_A1_T5()
        => CompilationFailureTest("S12.9_A1_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T6.js")]
    public Task test_S12_9_A1_T6()
        => CompilationFailureTest("S12.9_A1_T6", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T7.js")]
    public Task test_S12_9_A1_T7()
        => CompilationFailureTest("S12.9_A1_T7", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T8.js")]
    public Task test_S12_9_A1_T8()
        => CompilationFailureTest("S12.9_A1_T8", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A1_T9.js")]
    public Task test_S12_9_A1_T9()
        => CompilationFailureTest("S12.9_A1_T9", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.9_A3.js")]
    public Task test_S12_9_A3()
        => ExecutionTest("S12.9_A3");

    [Fact(DisplayName = "S12.9_A4.js")]
    public Task test_S12_9_A4()
        => ExecutionTest("S12.9_A4");

    [Fact(DisplayName = "S12.9_A5.js")]
    public Task test_S12_9_A5()
        => ExecutionTest("S12.9_A5");

    [Fact(DisplayName = "line-terminators.js")]
    public Task test_line_terminators()
        => ExecutionTest("line-terminators");

}
