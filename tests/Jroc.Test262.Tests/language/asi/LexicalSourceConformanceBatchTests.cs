using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.asi;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/asi", "language.asi") { }

    [Fact(DisplayName = "S7.9.2_A1_T1.js")]
    public Task S7_9_2_A1_T1()
        => CompilationFailureTest("S7.9.2_A1_T1");

    [Fact(DisplayName = "S7.9.2_A1_T2.js")]
    public Task S7_9_2_A1_T2()
        => ExecutionTest("S7.9.2_A1_T2");

    [Fact(DisplayName = "S7.9.2_A1_T3.js")]
    public Task S7_9_2_A1_T3()
        => CompilationFailureTest("S7.9.2_A1_T3");

    [Fact(DisplayName = "S7.9.2_A1_T4.js")]
    public Task S7_9_2_A1_T4()
        => ExecutionTest("S7.9.2_A1_T4");

    [Fact(DisplayName = "S7.9.2_A1_T5.js")]
    public Task S7_9_2_A1_T5()
        => ExecutionTest("S7.9.2_A1_T5");

    [Fact(DisplayName = "S7.9.2_A1_T6.js")]
    public Task S7_9_2_A1_T6()
        => CompilationFailureTest("S7.9.2_A1_T6");

    [Fact(DisplayName = "S7.9.2_A1_T7.js")]
    public Task S7_9_2_A1_T7()
        => ExecutionTest("S7.9.2_A1_T7");

    [Fact(DisplayName = "S7.9_A1.js")]
    public Task S7_9_A1()
        => ExecutionTest("S7.9_A1");

    [Fact(DisplayName = "S7.9_A10_T1.js")]
    public Task S7_9_A10_T1()
        => ExecutionTest("S7.9_A10_T1");

    [Fact(DisplayName = "S7.9_A10_T10.js")]
    public Task S7_9_A10_T10()
        => ExecutionTest("S7.9_A10_T10");

    [Fact(DisplayName = "S7.9_A10_T11.js")]
    public Task S7_9_A10_T11()
        => ExecutionTest("S7.9_A10_T11");

    [Fact(DisplayName = "S7.9_A10_T12.js")]
    public Task S7_9_A10_T12()
        => ExecutionTest("S7.9_A10_T12");

    [Fact(DisplayName = "S7.9_A10_T2.js")]
    public Task S7_9_A10_T2()
        => CompilationFailureTest("S7.9_A10_T2");

    [Fact(DisplayName = "S7.9_A10_T3.js")]
    public Task S7_9_A10_T3()
        => ExecutionTest("S7.9_A10_T3");

    [Fact(DisplayName = "S7.9_A10_T4.js")]
    public Task S7_9_A10_T4()
        => CompilationFailureTest("S7.9_A10_T4");

    [Fact(DisplayName = "S7.9_A10_T5.js")]
    public Task S7_9_A10_T5()
        => ExecutionTest("S7.9_A10_T5");

    [Fact(DisplayName = "S7.9_A10_T6.js")]
    public Task S7_9_A10_T6()
        => CompilationFailureTest("S7.9_A10_T6");

    [Fact(DisplayName = "S7.9_A10_T7.js")]
    public Task S7_9_A10_T7()
        => ExecutionTest("S7.9_A10_T7");

    [Fact(DisplayName = "S7.9_A10_T8.js")]
    public Task S7_9_A10_T8()
        => CompilationFailureTest("S7.9_A10_T8");

    [Fact(DisplayName = "S7.9_A10_T9.js")]
    public Task S7_9_A10_T9()
        => ExecutionTest("S7.9_A10_T9");

    [Fact(DisplayName = "S7.9_A11_T1.js")]
    public Task S7_9_A11_T1()
        => ExecutionTest("S7.9_A11_T1");

    [Fact(DisplayName = "S7.9_A11_T10.js")]
    public Task S7_9_A11_T10()
        => ExecutionTest("S7.9_A11_T10");

    [Fact(DisplayName = "S7.9_A11_T11.js")]
    public Task S7_9_A11_T11()
        => ExecutionTest("S7.9_A11_T11");

    [Fact(DisplayName = "S7.9_A11_T2.js")]
    public Task S7_9_A11_T2()
        => ExecutionTest("S7.9_A11_T2");

    [Fact(DisplayName = "S7.9_A11_T3.js")]
    public Task S7_9_A11_T3()
        => ExecutionTest("S7.9_A11_T3");

    [Fact(DisplayName = "S7.9_A11_T4.js")]
    public Task S7_9_A11_T4()
        => CompilationFailureTest("S7.9_A11_T4");

    [Fact(DisplayName = "S7.9_A11_T5.js")]
    public Task S7_9_A11_T5()
        => ExecutionTest("S7.9_A11_T5");

    [Fact(DisplayName = "S7.9_A11_T6.js")]
    public Task S7_9_A11_T6()
        => ExecutionTest("S7.9_A11_T6");

    [Fact(DisplayName = "S7.9_A11_T7.js")]
    public Task S7_9_A11_T7()
        => ExecutionTest("S7.9_A11_T7");

    [Fact(DisplayName = "S7.9_A11_T8.js")]
    public Task S7_9_A11_T8()
        => CompilationFailureTest("S7.9_A11_T8");

    [Fact(DisplayName = "S7.9_A11_T9.js")]
    public Task S7_9_A11_T9()
        => ExecutionTest("S7.9_A11_T9");

    [Fact(DisplayName = "S7.9_A2.js")]
    public Task S7_9_A2()
        => ExecutionTest("S7.9_A2");

    [Fact(DisplayName = "S7.9_A3.js")]
    public Task S7_9_A3()
        => ExecutionTest("S7.9_A3");

    [Fact(DisplayName = "S7.9_A4.js")]
    public Task S7_9_A4()
        => CompilationFailureTest("S7.9_A4");

    [Fact(DisplayName = "S7.9_A5.1_T1.js")]
    public Task S7_9_A5_1_T1()
        => CompilationFailureTest("S7.9_A5.1_T1");

    [Fact(DisplayName = "S7.9_A5.2_T1.js")]
    public Task S7_9_A5_2_T1()
        => ExecutionTest("S7.9_A5.2_T1");

    [Fact(DisplayName = "S7.9_A5.3_T1.js")]
    public Task S7_9_A5_3_T1()
        => CompilationFailureTest("S7.9_A5.3_T1");

    [Fact(DisplayName = "S7.9_A5.4_T1.js")]
    public Task S7_9_A5_4_T1()
        => ExecutionTest("S7.9_A5.4_T1");

    [Fact(DisplayName = "S7.9_A5.5_T1.js")]
    public Task S7_9_A5_5_T1()
        => ExecutionTest("S7.9_A5.5_T1");

    [Fact(DisplayName = "S7.9_A5.5_T2.js")]
    public Task S7_9_A5_5_T2()
        => ExecutionTest("S7.9_A5.5_T2");

    [Fact(DisplayName = "S7.9_A5.5_T3.js")]
    public Task S7_9_A5_5_T3()
        => ExecutionTest("S7.9_A5.5_T3");

    [Fact(DisplayName = "S7.9_A5.5_T4.js")]
    public Task S7_9_A5_5_T4()
        => ExecutionTest("S7.9_A5.5_T4");

    [Fact(DisplayName = "S7.9_A5.5_T5.js")]
    public Task S7_9_A5_5_T5()
        => ExecutionTest("S7.9_A5.5_T5");

    [Fact(DisplayName = "S7.9_A5.6_T1.js")]
    public Task S7_9_A5_6_T1()
        => ExecutionTest("S7.9_A5.6_T1");

    [Fact(DisplayName = "S7.9_A5.6_T2.js")]
    public Task S7_9_A5_6_T2()
        => ExecutionTest("S7.9_A5.6_T2");

    [Fact(DisplayName = "S7.9_A5.7_T1.js")]
    public Task S7_9_A5_7_T1()
        => CompilationFailureTest("S7.9_A5.7_T1");

    [Fact(DisplayName = "S7.9_A5.8_T1.js")]
    public Task S7_9_A5_8_T1()
        => ExecutionTest("S7.9_A5.8_T1");

    [Fact(DisplayName = "S7.9_A5.9_T1.js")]
    public Task S7_9_A5_9_T1()
        => ExecutionTest("S7.9_A5.9_T1");

    [Fact(DisplayName = "S7.9_A6.1_T1.js")]
    public Task S7_9_A6_1_T1()
        => ExecutionTest("S7.9_A6.1_T1");

    [Fact(DisplayName = "S7.9_A6.1_T10.js")]
    public Task S7_9_A6_1_T10()
        => ExecutionTest("S7.9_A6.1_T10");

    [Fact(DisplayName = "S7.9_A6.1_T11.js")]
    public Task S7_9_A6_1_T11()
        => ExecutionTest("S7.9_A6.1_T11");

    [Fact(DisplayName = "S7.9_A6.1_T12.js")]
    public Task S7_9_A6_1_T12()
        => ExecutionTest("S7.9_A6.1_T12");

    [Fact(DisplayName = "S7.9_A6.1_T13.js")]
    public Task S7_9_A6_1_T13()
        => ExecutionTest("S7.9_A6.1_T13");

    [Fact(DisplayName = "S7.9_A6.1_T2.js")]
    public Task S7_9_A6_1_T2()
        => ExecutionTest("S7.9_A6.1_T2");

    [Fact(DisplayName = "S7.9_A6.1_T3.js")]
    public Task S7_9_A6_1_T3()
        => ExecutionTest("S7.9_A6.1_T3");

    [Fact(DisplayName = "S7.9_A6.1_T4.js")]
    public Task S7_9_A6_1_T4()
        => ExecutionTest("S7.9_A6.1_T4");

    [Fact(DisplayName = "S7.9_A6.1_T5.js")]
    public Task S7_9_A6_1_T5()
        => ExecutionTest("S7.9_A6.1_T5");

    [Fact(DisplayName = "S7.9_A6.1_T6.js")]
    public Task S7_9_A6_1_T6()
        => ExecutionTest("S7.9_A6.1_T6");

    [Fact(DisplayName = "S7.9_A6.1_T7.js")]
    public Task S7_9_A6_1_T7()
        => ExecutionTest("S7.9_A6.1_T7");

    [Fact(DisplayName = "S7.9_A6.1_T8.js")]
    public Task S7_9_A6_1_T8()
        => ExecutionTest("S7.9_A6.1_T8");

    [Fact(DisplayName = "S7.9_A6.1_T9.js")]
    public Task S7_9_A6_1_T9()
        => ExecutionTest("S7.9_A6.1_T9");

    [Fact(DisplayName = "S7.9_A6.2_T1.js")]
    public Task S7_9_A6_2_T1()
        => CompilationFailureTest("S7.9_A6.2_T1");

    [Fact(DisplayName = "S7.9_A6.2_T10.js")]
    public Task S7_9_A6_2_T10()
        => CompilationFailureTest("S7.9_A6.2_T10");

    [Fact(DisplayName = "S7.9_A6.2_T2.js")]
    public Task S7_9_A6_2_T2()
        => CompilationFailureTest("S7.9_A6.2_T2");

    [Fact(DisplayName = "S7.9_A6.2_T3.js")]
    public Task S7_9_A6_2_T3()
        => CompilationFailureTest("S7.9_A6.2_T3");

    [Fact(DisplayName = "S7.9_A6.2_T4.js")]
    public Task S7_9_A6_2_T4()
        => CompilationFailureTest("S7.9_A6.2_T4");

    [Fact(DisplayName = "S7.9_A6.2_T5.js")]
    public Task S7_9_A6_2_T5()
        => CompilationFailureTest("S7.9_A6.2_T5");

    [Fact(DisplayName = "S7.9_A6.2_T6.js")]
    public Task S7_9_A6_2_T6()
        => CompilationFailureTest("S7.9_A6.2_T6");

    [Fact(DisplayName = "S7.9_A6.2_T7.js")]
    public Task S7_9_A6_2_T7()
        => CompilationFailureTest("S7.9_A6.2_T7");

    [Fact(DisplayName = "S7.9_A6.2_T8.js")]
    public Task S7_9_A6_2_T8()
        => CompilationFailureTest("S7.9_A6.2_T8");

    [Fact(DisplayName = "S7.9_A6.2_T9.js")]
    public Task S7_9_A6_2_T9()
        => CompilationFailureTest("S7.9_A6.2_T9");

    [Fact(DisplayName = "S7.9_A6.3_T1.js")]
    public Task S7_9_A6_3_T1()
        => CompilationFailureTest("S7.9_A6.3_T1");

    [Fact(DisplayName = "S7.9_A6.3_T2.js")]
    public Task S7_9_A6_3_T2()
        => CompilationFailureTest("S7.9_A6.3_T2");

    [Fact(DisplayName = "S7.9_A6.3_T3.js")]
    public Task S7_9_A6_3_T3()
        => CompilationFailureTest("S7.9_A6.3_T3");

    [Fact(DisplayName = "S7.9_A6.3_T4.js")]
    public Task S7_9_A6_3_T4()
        => CompilationFailureTest("S7.9_A6.3_T4");

    [Fact(DisplayName = "S7.9_A6.3_T5.js")]
    public Task S7_9_A6_3_T5()
        => CompilationFailureTest("S7.9_A6.3_T5");

    [Fact(DisplayName = "S7.9_A6.3_T6.js")]
    public Task S7_9_A6_3_T6()
        => CompilationFailureTest("S7.9_A6.3_T6");

    [Fact(DisplayName = "S7.9_A6.3_T7.js")]
    public Task S7_9_A6_3_T7()
        => CompilationFailureTest("S7.9_A6.3_T7");

    [Fact(DisplayName = "S7.9_A6.4_T1.js")]
    public Task S7_9_A6_4_T1()
        => CompilationFailureTest("S7.9_A6.4_T1");

    [Fact(DisplayName = "S7.9_A6.4_T2.js")]
    public Task S7_9_A6_4_T2()
        => CompilationFailureTest("S7.9_A6.4_T2");

    [Fact(DisplayName = "S7.9_A7_T1.js")]
    public Task S7_9_A7_T1()
        => ExecutionTest("S7.9_A7_T1");

    [Fact(DisplayName = "S7.9_A7_T2.js")]
    public Task S7_9_A7_T2()
        => ExecutionTest("S7.9_A7_T2");

    [Fact(DisplayName = "S7.9_A7_T3.js")]
    public Task S7_9_A7_T3()
        => ExecutionTest("S7.9_A7_T3");

    [Fact(DisplayName = "S7.9_A7_T4.js")]
    public Task S7_9_A7_T4()
        => ExecutionTest("S7.9_A7_T4");

    [Fact(DisplayName = "S7.9_A7_T5.js")]
    public Task S7_9_A7_T5()
        => ExecutionTest("S7.9_A7_T5");

    [Fact(DisplayName = "S7.9_A7_T6.js")]
    public Task S7_9_A7_T6()
        => ExecutionTest("S7.9_A7_T6");

    [Fact(DisplayName = "S7.9_A7_T7.js")]
    public Task S7_9_A7_T7()
        => ExecutionTest("S7.9_A7_T7");

    [Fact(DisplayName = "S7.9_A7_T8.js")]
    public Task S7_9_A7_T8()
        => ExecutionTest("S7.9_A7_T8");

    [Fact(DisplayName = "S7.9_A7_T9.js")]
    public Task S7_9_A7_T9()
        => ExecutionTest("S7.9_A7_T9");

    [Fact(DisplayName = "S7.9_A8_T1.js")]
    public Task S7_9_A8_T1()
        => ExecutionTest("S7.9_A8_T1");

    [Fact(DisplayName = "S7.9_A8_T2.js")]
    public Task S7_9_A8_T2()
        => ExecutionTest("S7.9_A8_T2");

    [Fact(DisplayName = "S7.9_A8_T3.js")]
    public Task S7_9_A8_T3()
        => ExecutionTest("S7.9_A8_T3");

    [Fact(DisplayName = "S7.9_A8_T4.js")]
    public Task S7_9_A8_T4()
        => ExecutionTest("S7.9_A8_T4");

    [Fact(DisplayName = "S7.9_A8_T5.js")]
    public Task S7_9_A8_T5()
        => ExecutionTest("S7.9_A8_T5");

    [Fact(DisplayName = "S7.9_A9_T1.js")]
    public Task S7_9_A9_T1()
        => ExecutionTest("S7.9_A9_T1");

    [Fact(DisplayName = "S7.9_A9_T2.js")]
    public Task S7_9_A9_T2()
        => ExecutionTest("S7.9_A9_T2");

    [Fact(DisplayName = "S7.9_A9_T5.js")]
    public Task S7_9_A9_T5()
        => ExecutionTest("S7.9_A9_T5");

    [Fact(DisplayName = "S7.9_A9_T6.js")]
    public Task S7_9_A9_T6()
        => CompilationFailureTest("S7.9_A9_T6");

    [Fact(DisplayName = "S7.9_A9_T7.js")]
    public Task S7_9_A9_T7()
        => CompilationFailureTest("S7.9_A9_T7");

    [Fact(DisplayName = "S7.9_A9_T8.js")]
    public Task S7_9_A9_T8()
        => CompilationFailureTest("S7.9_A9_T8");

    [Fact(DisplayName = "S7.9_A9_T9.js")]
    public Task S7_9_A9_T9()
        => ExecutionTest("S7.9_A9_T9");

    [Fact(DisplayName = "do-while-same-line.js")]
    public Task do_while_same_line()
        => ExecutionTest("do-while-same-line");
}
