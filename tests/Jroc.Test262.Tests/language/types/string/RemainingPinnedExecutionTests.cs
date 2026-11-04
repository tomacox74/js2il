namespace Jroc.Test262.Tests.language.types.@string;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.string") { }

    [Fact(DisplayName = "S8.4_A13_T1.js")]
    public Task S8_4_A13_T1()
        => CompilationFailureTest("S8.4_A13_T1");

    [Fact(DisplayName = "S8.4_A13_T2.js")]
    public Task S8_4_A13_T2()
        => CompilationFailureTest("S8.4_A13_T2");

    [Fact(DisplayName = "S8.4_A13_T3.js")]
    public Task S8_4_A13_T3()
        => CompilationFailureTest("S8.4_A13_T3");

    [Fact(DisplayName = "S8.4_A14_T1.js")]
    public Task S8_4_A14_T1()
        => CompilationFailureTest("S8.4_A14_T1");

    [Fact(DisplayName = "S8.4_A14_T2.js")]
    public Task S8_4_A14_T2()
        => CompilationFailureTest("S8.4_A14_T2");

    [Fact(DisplayName = "S8.4_A14_T3.js")]
    public Task S8_4_A14_T3()
        => CompilationFailureTest("S8.4_A14_T3");

    [Fact(DisplayName = "S8.4_A7.1.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S8_4_A7_1()
        => ExecutionTest("S8.4_A7.1");

    [Fact(DisplayName = "S8.4_A7.2.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S8_4_A7_2()
        => ExecutionTest("S8.4_A7.2");

    [Fact(DisplayName = "S8.4_A7.3.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S8_4_A7_3()
        => ExecutionTest("S8.4_A7.3");

    [Fact(DisplayName = "S8.4_A7.4.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task S8_4_A7_4()
        => ExecutionTest("S8.4_A7.4");

}
