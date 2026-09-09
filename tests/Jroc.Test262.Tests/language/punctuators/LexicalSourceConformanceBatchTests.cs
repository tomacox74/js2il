using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.punctuators;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/punctuators", "language.punctuators") { }

    [Fact(DisplayName = "S7.7_A1.js")]
    public Task S7_7_A1()
        => ExecutionTest("S7.7_A1");

    [Fact(DisplayName = "S7.7_A2_T1.js")]
    public Task S7_7_A2_T1()
        => CompilationFailureTest("S7.7_A2_T1");

    [Fact(DisplayName = "S7.7_A2_T10.js")]
    public Task S7_7_A2_T10()
        => CompilationFailureTest("S7.7_A2_T10");

    [Fact(DisplayName = "S7.7_A2_T2.js")]
    public Task S7_7_A2_T2()
        => CompilationFailureTest("S7.7_A2_T2");

    [Fact(DisplayName = "S7.7_A2_T3.js")]
    public Task S7_7_A2_T3()
        => CompilationFailureTest("S7.7_A2_T3");

    [Fact(DisplayName = "S7.7_A2_T4.js")]
    public Task S7_7_A2_T4()
        => CompilationFailureTest("S7.7_A2_T4");

    [Fact(DisplayName = "S7.7_A2_T5.js")]
    public Task S7_7_A2_T5()
        => CompilationFailureTest("S7.7_A2_T5");

    [Fact(DisplayName = "S7.7_A2_T6.js")]
    public Task S7_7_A2_T6()
        => CompilationFailureTest("S7.7_A2_T6");

    [Fact(DisplayName = "S7.7_A2_T7.js")]
    public Task S7_7_A2_T7()
        => CompilationFailureTest("S7.7_A2_T7");

    [Fact(DisplayName = "S7.7_A2_T8.js")]
    public Task S7_7_A2_T8()
        => CompilationFailureTest("S7.7_A2_T8");

    [Fact(DisplayName = "S7.7_A2_T9.js")]
    public Task S7_7_A2_T9()
        => CompilationFailureTest("S7.7_A2_T9");
}
