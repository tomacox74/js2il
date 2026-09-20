using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.instanceof;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.instanceof") { }

    [Fact(DisplayName = "S11.8.6_A2.4_T2")]
    public Task S11_8_6_A2_4_T2()
        => ExecutionTest("S11.8.6_A2.4_T2");

    [Fact(DisplayName = "S11.8.6_A2.4_T3")]
    public Task S11_8_6_A2_4_T3()
        => ExecutionTest("S11.8.6_A2.4_T3");

    [Fact(DisplayName = "S11.8.6_A2.4_T4")]
    public Task S11_8_6_A2_4_T4()
        => ExecutionTest("S11.8.6_A2.4_T4");

    [Fact(DisplayName = "S11.8.6_A4_T1")]
    public Task S11_8_6_A4_T1()
        => ExecutionTest("S11.8.6_A4_T1");

    [Fact(DisplayName = "S11.8.6_A4_T2")]
    public Task S11_8_6_A4_T2()
        => ExecutionTest("S11.8.6_A4_T2");

    [Fact(DisplayName = "S11.8.6_A4_T3")]
    public Task S11_8_6_A4_T3()
        => ExecutionTest("S11.8.6_A4_T3");

    [Fact(DisplayName = "S11.8.6_A5_T1")]
    public Task S11_8_6_A5_T1()
        => ExecutionTest("S11.8.6_A5_T1");

    [Fact(DisplayName = "S11.8.6_A5_T2")]
    public Task S11_8_6_A5_T2()
        => ExecutionTest("S11.8.6_A5_T2");

    [Fact(DisplayName = "S11.8.6_A6_T1")]
    public Task S11_8_6_A6_T1()
        => ExecutionTest("S11.8.6_A6_T1");

    [Fact(DisplayName = "S11.8.6_A6_T2")]
    public Task S11_8_6_A6_T2()
        => ExecutionTest("S11.8.6_A6_T2");

    [Fact(DisplayName = "S11.8.6_A6_T3")]
    public Task S11_8_6_A6_T3()
        => ExecutionTest("S11.8.6_A6_T3");

    [Fact(DisplayName = "S11.8.6_A6_T4")]
    public Task S11_8_6_A6_T4()
        => ExecutionTest("S11.8.6_A6_T4");

    [Fact(DisplayName = "S11.8.6_A7_T1")]
    public Task S11_8_6_A7_T1()
        => ExecutionTest("S11.8.6_A7_T1");

    [Fact(DisplayName = "S11.8.6_A7_T2")]
    public Task S11_8_6_A7_T2()
        => ExecutionTest("S11.8.6_A7_T2");

    [Fact(DisplayName = "S11.8.6_A7_T3")]
    public Task S11_8_6_A7_T3()
        => ExecutionTest("S11.8.6_A7_T3");

    [Fact(DisplayName = "S15.3.5.3_A1_T1")]
    public Task S15_3_5_3_A1_T1()
        => ExecutionTest("S15.3.5.3_A1_T1");

    [Fact(DisplayName = "S15.3.5.3_A1_T2")]
    public Task S15_3_5_3_A1_T2()
        => ExecutionTest("S15.3.5.3_A1_T2");

    [Fact(DisplayName = "S15.3.5.3_A1_T3")]
    public Task S15_3_5_3_A1_T3()
        => ExecutionTest("S15.3.5.3_A1_T3");

    [Fact(DisplayName = "S15.3.5.3_A1_T4")]
    public Task S15_3_5_3_A1_T4()
        => ExecutionTest("S15.3.5.3_A1_T4");

    [Fact(DisplayName = "S15.3.5.3_A1_T5")]
    public Task S15_3_5_3_A1_T5()
        => ExecutionTest("S15.3.5.3_A1_T5");

    [Fact(DisplayName = "S15.3.5.3_A1_T6")]
    public Task S15_3_5_3_A1_T6()
        => ExecutionTest("S15.3.5.3_A1_T6");

    [Fact(DisplayName = "S15.3.5.3_A1_T7")]
    public Task S15_3_5_3_A1_T7()
        => ExecutionTest("S15.3.5.3_A1_T7");

    [Fact(DisplayName = "S15.3.5.3_A1_T8")]
    public Task S15_3_5_3_A1_T8()
        => ExecutionTest("S15.3.5.3_A1_T8");

    [Fact(DisplayName = "S15.3.5.3_A2_T2")]
    public Task S15_3_5_3_A2_T2()
        => ExecutionTest("S15.3.5.3_A2_T2");

    [Fact(DisplayName = "S15.3.5.3_A2_T5")]
    public Task S15_3_5_3_A2_T5()
        => ExecutionTest("S15.3.5.3_A2_T5");

    [Fact(DisplayName = "S15.3.5.3_A2_T6")]
    public Task S15_3_5_3_A2_T6()
        => ExecutionTest("S15.3.5.3_A2_T6");

    [Fact(DisplayName = "S15.3.5.3_A3_T1")]
    public Task S15_3_5_3_A3_T1()
        => ExecutionTest("S15.3.5.3_A3_T1");

    [Fact(DisplayName = "S15.3.5.3_A3_T2")]
    public Task S15_3_5_3_A3_T2()
        => ExecutionTest("S15.3.5.3_A3_T2");

    [Fact(DisplayName = "primitive-prototype-with-object")]
    public Task primitive_prototype_with_object()
        => ExecutionTest("primitive-prototype-with-object");

    [Fact(DisplayName = "primitive-prototype-with-primitive")]
    public Task primitive_prototype_with_primitive()
        => ExecutionTest("primitive-prototype-with-primitive");

    [Fact(DisplayName = "prototype-getter-with-object-throws")]
    public Task prototype_getter_with_object_throws()
        => ExecutionTest("prototype-getter-with-object-throws");

    [Fact(DisplayName = "prototype-getter-with-object")]
    public Task prototype_getter_with_object()
        => ExecutionTest("prototype-getter-with-object");

    [Fact(DisplayName = "prototype-getter-with-primitive")]
    public Task prototype_getter_with_primitive()
        => ExecutionTest("prototype-getter-with-primitive");

    [Fact(DisplayName = "symbol-hasinstance-not-callable")]
    public Task symbol_hasinstance_not_callable()
        => ExecutionTest("symbol-hasinstance-not-callable");

}
