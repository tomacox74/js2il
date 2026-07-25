using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.numeric;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.literals.numeric") { }

    [Fact(DisplayName = "binary")]
    public Task binary()
        => ExecutionTest("binary");

    [Fact(DisplayName = "legacy-octal-integer")]
    public Task legacy_octal_integer()
        => ExecutionTest("legacy-octal-integer");

    [Fact(DisplayName = "non-octal-decimal-integer")]
    public Task non_octal_decimal_integer()
        => ExecutionTest("non-octal-decimal-integer");

    [Fact(DisplayName = "octal")]
    public Task octal()
        => ExecutionTest("octal");

    [Fact(DisplayName = "S7.8.3_A1.1_T1")]
    public Task S7_8_3_A1_1_T1()
        => ExecutionTest("S7.8.3_A1.1_T1");

    [Fact(DisplayName = "S7.8.3_A1.1_T2")]
    public Task S7_8_3_A1_1_T2()
        => ExecutionTest("S7.8.3_A1.1_T2");

    [Fact(DisplayName = "S7.8.3_A1.2_T1")]
    public Task S7_8_3_A1_2_T1()
        => ExecutionTest("S7.8.3_A1.2_T1");

    [Fact(DisplayName = "S7.8.3_A1.2_T2")]
    public Task S7_8_3_A1_2_T2()
        => ExecutionTest("S7.8.3_A1.2_T2");

    [Fact(DisplayName = "S7.8.3_A1.2_T3")]
    public Task S7_8_3_A1_2_T3()
        => ExecutionTest("S7.8.3_A1.2_T3");

    [Fact(DisplayName = "S7.8.3_A1.2_T4")]
    public Task S7_8_3_A1_2_T4()
        => ExecutionTest("S7.8.3_A1.2_T4");

    [Fact(DisplayName = "S7.8.3_A1.2_T5")]
    public Task S7_8_3_A1_2_T5()
        => ExecutionTest("S7.8.3_A1.2_T5");

    [Fact(DisplayName = "S7.8.3_A1.2_T6")]
    public Task S7_8_3_A1_2_T6()
        => ExecutionTest("S7.8.3_A1.2_T6");

    [Fact(DisplayName = "S7.8.3_A1.2_T7")]
    public Task S7_8_3_A1_2_T7()
        => ExecutionTest("S7.8.3_A1.2_T7");

    [Fact(DisplayName = "S7.8.3_A1.2_T8")]
    public Task S7_8_3_A1_2_T8()
        => ExecutionTest("S7.8.3_A1.2_T8");

    [Fact(DisplayName = "S7.8.3_A2.1_T1")]
    public Task S7_8_3_A2_1_T1()
        => ExecutionTest("S7.8.3_A2.1_T1");

    [Fact(DisplayName = "S7.8.3_A2.1_T2")]
    public Task S7_8_3_A2_1_T2()
        => ExecutionTest("S7.8.3_A2.1_T2");

    [Fact(DisplayName = "S7.8.3_A2.1_T3")]
    public Task S7_8_3_A2_1_T3()
        => ExecutionTest("S7.8.3_A2.1_T3");

    [Fact(DisplayName = "S7.8.3_A2.2_T1")]
    public Task S7_8_3_A2_2_T1()
        => ExecutionTest("S7.8.3_A2.2_T1");

    [Fact(DisplayName = "S7.8.3_A2.2_T2")]
    public Task S7_8_3_A2_2_T2()
        => ExecutionTest("S7.8.3_A2.2_T2");

    [Fact(DisplayName = "S7.8.3_A2.2_T3")]
    public Task S7_8_3_A2_2_T3()
        => ExecutionTest("S7.8.3_A2.2_T3");

    [Fact(DisplayName = "S7.8.3_A2.2_T4")]
    public Task S7_8_3_A2_2_T4()
        => ExecutionTest("S7.8.3_A2.2_T4");

    [Fact(DisplayName = "S7.8.3_A2.2_T5")]
    public Task S7_8_3_A2_2_T5()
        => ExecutionTest("S7.8.3_A2.2_T5");

    [Fact(DisplayName = "S7.8.3_A2.2_T6")]
    public Task S7_8_3_A2_2_T6()
        => ExecutionTest("S7.8.3_A2.2_T6");

    [Fact(DisplayName = "S7.8.3_A2.2_T7")]
    public Task S7_8_3_A2_2_T7()
        => ExecutionTest("S7.8.3_A2.2_T7");

    [Fact(DisplayName = "S7.8.3_A2.2_T8")]
    public Task S7_8_3_A2_2_T8()
        => ExecutionTest("S7.8.3_A2.2_T8");

    [Fact(DisplayName = "S7.8.3_A3.1_T1")]
    public Task S7_8_3_A3_1_T1()
        => ExecutionTest("S7.8.3_A3.1_T1");

    [Fact(DisplayName = "S7.8.3_A3.1_T2")]
    public Task S7_8_3_A3_1_T2()
        => ExecutionTest("S7.8.3_A3.1_T2");

    [Fact(DisplayName = "S7.8.3_A3.2_T1")]
    public Task S7_8_3_A3_2_T1()
        => ExecutionTest("S7.8.3_A3.2_T1");

    [Fact(DisplayName = "S7.8.3_A3.2_T2")]
    public Task S7_8_3_A3_2_T2()
        => ExecutionTest("S7.8.3_A3.2_T2");

    [Fact(DisplayName = "S7.8.3_A3.2_T3")]
    public Task S7_8_3_A3_2_T3()
        => ExecutionTest("S7.8.3_A3.2_T3");

    [Fact(DisplayName = "S7.8.3_A3.3_T1")]
    public Task S7_8_3_A3_3_T1()
        => ExecutionTest("S7.8.3_A3.3_T1");

    [Fact(DisplayName = "S7.8.3_A3.3_T2")]
    public Task S7_8_3_A3_3_T2()
        => ExecutionTest("S7.8.3_A3.3_T2");

    [Fact(DisplayName = "S7.8.3_A3.3_T3")]
    public Task S7_8_3_A3_3_T3()
        => ExecutionTest("S7.8.3_A3.3_T3");

    [Fact(DisplayName = "S7.8.3_A3.3_T4")]
    public Task S7_8_3_A3_3_T4()
        => ExecutionTest("S7.8.3_A3.3_T4");

    [Fact(DisplayName = "S7.8.3_A3.3_T5")]
    public Task S7_8_3_A3_3_T5()
        => ExecutionTest("S7.8.3_A3.3_T5");

    [Fact(DisplayName = "S7.8.3_A3.3_T6")]
    public Task S7_8_3_A3_3_T6()
        => ExecutionTest("S7.8.3_A3.3_T6");

    [Fact(DisplayName = "S7.8.3_A3.3_T7")]
    public Task S7_8_3_A3_3_T7()
        => ExecutionTest("S7.8.3_A3.3_T7");

    [Fact(DisplayName = "S7.8.3_A3.3_T8")]
    public Task S7_8_3_A3_3_T8()
        => ExecutionTest("S7.8.3_A3.3_T8");

    [Fact(DisplayName = "S7.8.3_A3.4_T1")]
    public Task S7_8_3_A3_4_T1()
        => ExecutionTest("S7.8.3_A3.4_T1");

    [Fact(DisplayName = "S7.8.3_A3.4_T2")]
    public Task S7_8_3_A3_4_T2()
        => ExecutionTest("S7.8.3_A3.4_T2");

    [Fact(DisplayName = "S7.8.3_A3.4_T3")]
    public Task S7_8_3_A3_4_T3()
        => ExecutionTest("S7.8.3_A3.4_T3");

    [Fact(DisplayName = "S7.8.3_A3.4_T4")]
    public Task S7_8_3_A3_4_T4()
        => ExecutionTest("S7.8.3_A3.4_T4");

    [Fact(DisplayName = "S7.8.3_A3.4_T5")]
    public Task S7_8_3_A3_4_T5()
        => ExecutionTest("S7.8.3_A3.4_T5");

    [Fact(DisplayName = "S7.8.3_A3.4_T6")]
    public Task S7_8_3_A3_4_T6()
        => ExecutionTest("S7.8.3_A3.4_T6");

    [Fact(DisplayName = "S7.8.3_A3.4_T7")]
    public Task S7_8_3_A3_4_T7()
        => ExecutionTest("S7.8.3_A3.4_T7");

    [Fact(DisplayName = "S7.8.3_A3.4_T8")]
    public Task S7_8_3_A3_4_T8()
        => ExecutionTest("S7.8.3_A3.4_T8");

    [Fact(DisplayName = "S7.8.3_A4.1_T1")]
    public Task S7_8_3_A4_1_T1()
        => ExecutionTest("S7.8.3_A4.1_T1");

    [Fact(DisplayName = "S7.8.3_A4.1_T2")]
    public Task S7_8_3_A4_1_T2()
        => ExecutionTest("S7.8.3_A4.1_T2");

    [Fact(DisplayName = "S7.8.3_A4.1_T3")]
    public Task S7_8_3_A4_1_T3()
        => ExecutionTest("S7.8.3_A4.1_T3");

    [Fact(DisplayName = "S7.8.3_A4.1_T4")]
    public Task S7_8_3_A4_1_T4()
        => ExecutionTest("S7.8.3_A4.1_T4");

    [Fact(DisplayName = "S7.8.3_A4.1_T5")]
    public Task S7_8_3_A4_1_T5()
        => ExecutionTest("S7.8.3_A4.1_T5");

    [Fact(DisplayName = "S7.8.3_A4.1_T6")]
    public Task S7_8_3_A4_1_T6()
        => ExecutionTest("S7.8.3_A4.1_T6");

    [Fact(DisplayName = "S7.8.3_A4.1_T7")]
    public Task S7_8_3_A4_1_T7()
        => ExecutionTest("S7.8.3_A4.1_T7");

    [Fact(DisplayName = "S7.8.3_A4.1_T8")]
    public Task S7_8_3_A4_1_T8()
        => ExecutionTest("S7.8.3_A4.1_T8");
}
