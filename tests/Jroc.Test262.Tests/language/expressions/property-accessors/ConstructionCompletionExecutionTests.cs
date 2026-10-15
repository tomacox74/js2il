using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.property_accessors;

public class ConstructionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructionCompletionExecutionTests() : base("language.expressions.property_accessors") { }

    [Fact(DisplayName = "non-identifier-name.js")]
    public Task test_non_identifier_name() => CompilationFailureTest("non-identifier-name", string.Empty);

    [Fact(DisplayName = "S11.2.1_A1.1.js", Skip = "eval is not supported.")]
    public Task test_S11_2_1_A1_1() => ExecutionTest("S11.2.1_A1.1");

    [Fact(DisplayName = "S11.2.1_A1.2.js", Skip = "eval is not supported.")]
    public Task test_S11_2_1_A1_2() => ExecutionTest("S11.2.1_A1.2");

    [Fact(DisplayName = "S11.2.1_A2.js")]
    public Task test_S11_2_1_A2() => ExecutionTest("S11.2.1_A2");

    [Fact(DisplayName = "S11.2.1_A3_T1.js")]
    public Task test_S11_2_1_A3_T1() => ExecutionTest("S11.2.1_A3_T1");

    [Fact(DisplayName = "S11.2.1_A3_T2.js")]
    public Task test_S11_2_1_A3_T2() => ExecutionTest("S11.2.1_A3_T2");

    [Fact(DisplayName = "S11.2.1_A3_T3.js")]
    public Task test_S11_2_1_A3_T3() => ExecutionTest("S11.2.1_A3_T3");

    [Fact(DisplayName = "S11.2.1_A3_T4.js")]
    public Task test_S11_2_1_A3_T4() => ExecutionTest("S11.2.1_A3_T4");

    [Fact(DisplayName = "S11.2.1_A3_T5.js")]
    public Task test_S11_2_1_A3_T5() => ExecutionTest("S11.2.1_A3_T5");

    [Fact(DisplayName = "S11.2.1_A4_T1.js")]
    public Task test_S11_2_1_A4_T1() => ExecutionTest("S11.2.1_A4_T1");

    [Fact(DisplayName = "S11.2.1_A4_T2.js")]
    public Task test_S11_2_1_A4_T2() => ExecutionTest("S11.2.1_A4_T2");

    [Fact(DisplayName = "S11.2.1_A4_T6.js")]
    public Task test_S11_2_1_A4_T6() => ExecutionTest("S11.2.1_A4_T6");

    [Fact(DisplayName = "S11.2.1_A4_T7.js")]
    public Task test_S11_2_1_A4_T7() => ExecutionTest("S11.2.1_A4_T7");

    [Fact(DisplayName = "S11.2.1_A4_T8.js")]
    public Task test_S11_2_1_A4_T8() => ExecutionTest("S11.2.1_A4_T8");

    [Fact(DisplayName = "S11.2.1_A4_T9.js")]
    public Task test_S11_2_1_A4_T9() => ExecutionTest("S11.2.1_A4_T9");

    [Fact(DisplayName = "S8.12.3_A1.js")]
    public Task test_S8_12_3_A1() => ExecutionTest("S8.12.3_A1");

    [Fact(DisplayName = "S8.12.3_A2.js")]
    public Task test_S8_12_3_A2() => ExecutionTest("S8.12.3_A2");

    [Fact(DisplayName = "S8.12.3_A3.js")]
    public Task test_S8_12_3_A3() => ExecutionTest("S8.12.3_A3");
}
