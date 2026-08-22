using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number;

public class PortNext200ExecutionTests : DiskExecutionTestsBase
{
    public PortNext200ExecutionTests() : base("built_ins.Number") { }

    [Fact(DisplayName = "MAX_VALUE/S15.7.3.2_A4")]
    public Task MAX_VALUE_S15_7_3_2_A4()
        => ExecutionTestFromFile("MAX_VALUE/S15.7.3.2_A4");

    [Fact(DisplayName = "MIN_VALUE/S15.7.3.3_A4")]
    public Task MIN_VALUE_S15_7_3_3_A4()
        => ExecutionTestFromFile("MIN_VALUE/S15.7.3.3_A4");

    [Fact(DisplayName = "NEGATIVE_INFINITY/S15.7.3.5_A1")]
    public Task NEGATIVE_INFINITY_S15_7_3_5_A1()
        => ExecutionTestFromFile("NEGATIVE_INFINITY/S15.7.3.5_A1");

    [Fact(DisplayName = "POSITIVE_INFINITY/S15.7.3.6_A1")]
    public Task POSITIVE_INFINITY_S15_7_3_6_A1()
        => ExecutionTestFromFile("POSITIVE_INFINITY/S15.7.3.6_A1");

    [Fact(DisplayName = "S15.7.1.1_A2")]
    public Task S15_7_1_1_A2()
        => ExecutionTestFromFile("S15.7.1.1_A2");

    [Fact(DisplayName = "S15.7.2.1_A3")]
    public Task S15_7_2_1_A3()
        => ExecutionTestFromFile("S15.7.2.1_A3");

    [Fact(DisplayName = "S15.7.2.1_A4")]
    public Task S15_7_2_1_A4()
        => ExecutionTestFromFile("S15.7.2.1_A4");

    [Fact(DisplayName = "S15.7.3_A5")]
    public Task S15_7_3_A5()
        => ExecutionTestFromFile("S15.7.3_A5");

    [Fact(DisplayName = "S15.7.3_A6")]
    public Task S15_7_3_A6()
        => ExecutionTestFromFile("S15.7.3_A6");

    [Fact(DisplayName = "S15.7.3_A7")]
    public Task S15_7_3_A7()
        => ExecutionTestFromFile("S15.7.3_A7");

    [Fact(DisplayName = "S15.7.3_A8")]
    public Task S15_7_3_A8()
        => ExecutionTestFromFile("S15.7.3_A8");

    [Fact(DisplayName = "S15.7.5_A1_T03")]
    public Task S15_7_5_A1_T03()
        => ExecutionTestFromFile("S15.7.5_A1_T03");

    [Fact(DisplayName = "S15.7.5_A1_T04")]
    public Task S15_7_5_A1_T04()
        => ExecutionTestFromFile("S15.7.5_A1_T04");

    [Fact(DisplayName = "S15.7.5_A1_T05")]
    public Task S15_7_5_A1_T05()
        => ExecutionTestFromFile("S15.7.5_A1_T05");

    [Fact(DisplayName = "S15.7.5_A1_T06")]
    public Task S15_7_5_A1_T06()
        => ExecutionTestFromFile("S15.7.5_A1_T06");

    [Fact(DisplayName = "S15.7.5_A1_T07")]
    public Task S15_7_5_A1_T07()
        => ExecutionTestFromFile("S15.7.5_A1_T07");

    [Fact(DisplayName = "S8.12.8_A3")]
    public Task S8_12_8_A3()
        => ExecutionTestFromFile("S8.12.8_A3");

    [Fact(DisplayName = "S8.12.8_A4")]
    public Task S8_12_8_A4()
        => ExecutionTestFromFile("S8.12.8_A4");

    [Fact(DisplayName = "S9.1_A1_T1")]
    public Task S9_1_A1_T1()
        => ExecutionTestFromFile("S9.1_A1_T1");

    [Fact(DisplayName = "S9.3.1_A10")]
    public Task S9_3_1_A10()
        => ExecutionTestFromFile("S9.3.1_A10");

    [Fact(DisplayName = "S9.3.1_A11")]
    public Task S9_3_1_A11()
        => ExecutionTestFromFile("S9.3.1_A11");

    [Fact(DisplayName = "S9.3.1_A12")]
    public Task S9_3_1_A12()
        => ExecutionTestFromFile("S9.3.1_A12");

    [Fact(DisplayName = "S9.3.1_A13")]
    public Task S9_3_1_A13()
        => ExecutionTestFromFile("S9.3.1_A13");

    [Fact(DisplayName = "S9.3.1_A14")]
    public Task S9_3_1_A14()
        => ExecutionTestFromFile("S9.3.1_A14");

    [Fact(DisplayName = "S9.3.1_A15")]
    public Task S9_3_1_A15()
        => ExecutionTestFromFile("S9.3.1_A15");

    [Fact(DisplayName = "S9.3.1_A16")]
    public Task S9_3_1_A16()
        => ExecutionTestFromFile("S9.3.1_A16");

    [Fact(DisplayName = "S9.3.1_A18")]
    public Task S9_3_1_A18()
        => ExecutionTestFromFile("S9.3.1_A18");

    [Fact(DisplayName = "S9.3.1_A19")]
    public Task S9_3_1_A19()
        => ExecutionTestFromFile("S9.3.1_A19");

    [Fact(DisplayName = "S9.3.1_A2")]
    public Task S9_3_1_A2()
        => ExecutionTestFromFile("S9.3.1_A2");

    [Fact(DisplayName = "S9.3.1_A20")]
    public Task S9_3_1_A20()
        => ExecutionTestFromFile("S9.3.1_A20");

    [Fact(DisplayName = "S9.3.1_A21")]
    public Task S9_3_1_A21()
        => ExecutionTestFromFile("S9.3.1_A21");

    [Fact(DisplayName = "S9.3.1_A22")]
    public Task S9_3_1_A22()
        => ExecutionTestFromFile("S9.3.1_A22");

    [Fact(DisplayName = "S9.3.1_A23")]
    public Task S9_3_1_A23()
        => ExecutionTestFromFile("S9.3.1_A23");

    [Fact(DisplayName = "S9.3.1_A24")]
    public Task S9_3_1_A24()
        => ExecutionTestFromFile("S9.3.1_A24");

    [Fact(DisplayName = "S9.3.1_A25")]
    public Task S9_3_1_A25()
        => ExecutionTestFromFile("S9.3.1_A25");

    [Fact(DisplayName = "S9.3.1_A26")]
    public Task S9_3_1_A26()
        => ExecutionTestFromFile("S9.3.1_A26");

    [Fact(DisplayName = "S9.3.1_A27")]
    public Task S9_3_1_A27()
        => ExecutionTestFromFile("S9.3.1_A27");

    [Fact(DisplayName = "S9.3.1_A28")]
    public Task S9_3_1_A28()
        => ExecutionTestFromFile("S9.3.1_A28");

    [Fact(DisplayName = "S9.3.1_A29")]
    public Task S9_3_1_A29()
        => ExecutionTestFromFile("S9.3.1_A29");

    [Fact(DisplayName = "S9.3.1_A2_U180E")]
    public Task S9_3_1_A2_U180E()
        => ExecutionTestFromFile("S9.3.1_A2_U180E");

    [Fact(DisplayName = "S9.3.1_A30")]
    public Task S9_3_1_A30()
        => ExecutionTestFromFile("S9.3.1_A30");

    [Fact(DisplayName = "S9.3.1_A31")]
    public Task S9_3_1_A31()
        => ExecutionTestFromFile("S9.3.1_A31");

    [Fact(DisplayName = "S9.3.1_A32")]
    public Task S9_3_1_A32()
        => ExecutionTestFromFile("S9.3.1_A32");

    [Fact(DisplayName = "S9.3.1_A3_T1")]
    public Task S9_3_1_A3_T1()
        => ExecutionTestFromFile("S9.3.1_A3_T1");

    [Fact(DisplayName = "S9.3.1_A3_T1_U180E")]
    public Task S9_3_1_A3_T1_U180E()
        => ExecutionTestFromFile("S9.3.1_A3_T1_U180E");

    [Fact(DisplayName = "S9.3.1_A3_T2")]
    public Task S9_3_1_A3_T2()
        => ExecutionTestFromFile("S9.3.1_A3_T2");

    [Fact(DisplayName = "S9.3.1_A3_T2_U180E")]
    public Task S9_3_1_A3_T2_U180E()
        => ExecutionTestFromFile("S9.3.1_A3_T2_U180E");

    [Fact(DisplayName = "S9.3.1_A4_T1")]
    public Task S9_3_1_A4_T1()
        => ExecutionTestFromFile("S9.3.1_A4_T1");

    [Fact(DisplayName = "S9.3.1_A4_T2")]
    public Task S9_3_1_A4_T2()
        => ExecutionTestFromFile("S9.3.1_A4_T2");

    [Fact(DisplayName = "S9.3.1_A5_T1")]
    public Task S9_3_1_A5_T1()
        => ExecutionTestFromFile("S9.3.1_A5_T1");

    [Fact(DisplayName = "S9.3.1_A5_T2")]
    public Task S9_3_1_A5_T2()
        => ExecutionTestFromFile("S9.3.1_A5_T2");

    [Fact(DisplayName = "S9.3.1_A5_T3")]
    public Task S9_3_1_A5_T3()
        => ExecutionTestFromFile("S9.3.1_A5_T3");

    [Fact(DisplayName = "S9.3.1_A6_T1")]
    public Task S9_3_1_A6_T1()
        => ExecutionTestFromFile("S9.3.1_A6_T1");

    [Fact(DisplayName = "S9.3.1_A6_T2")]
    public Task S9_3_1_A6_T2()
        => ExecutionTestFromFile("S9.3.1_A6_T2");

    [Fact(DisplayName = "S9.3.1_A8")]
    public Task S9_3_1_A8()
        => ExecutionTestFromFile("S9.3.1_A8");

    [Fact(DisplayName = "S9.3.1_A9")]
    public Task S9_3_1_A9()
        => ExecutionTestFromFile("S9.3.1_A9");

    [Fact(DisplayName = "S9.3_A4.2_T1")]
    public Task S9_3_A4_2_T1()
        => ExecutionTestFromFile("S9.3_A4.2_T1");

    [Fact(DisplayName = "S9.3_A5_T1")]
    public Task S9_3_A5_T1()
        => ExecutionTestFromFile("S9.3_A5_T1");

    [Fact(DisplayName = "bigint-conversion")]
    public Task bigint_conversion()
        => ExecutionTestFromFile("bigint-conversion");

    [Fact(DisplayName = "isSafeInteger/arg-is-not-number")]
    public Task isSafeInteger_arg_is_not_number()
        => ExecutionTestFromFile("isSafeInteger/arg-is-not-number");

    [Fact(DisplayName = "isSafeInteger/infinity")]
    public Task isSafeInteger_infinity()
        => ExecutionTestFromFile("isSafeInteger/infinity");

    [Fact(DisplayName = "isSafeInteger/nan")]
    public Task isSafeInteger_nan()
        => ExecutionTestFromFile("isSafeInteger/nan");

    [Fact(DisplayName = "isSafeInteger/not-integer")]
    public Task isSafeInteger_not_integer()
        => ExecutionTestFromFile("isSafeInteger/not-integer");

    [Fact(DisplayName = "prototype/S15.7.3.1_A2_T1")]
    public Task prototype_S15_7_3_1_A2_T1()
        => ExecutionTestFromFile("prototype/S15.7.3.1_A2_T1");

    [Fact(DisplayName = "prototype/S15.7.3.1_A2_T2")]
    public Task prototype_S15_7_3_1_A2_T2()
        => ExecutionTestFromFile("prototype/S15.7.3.1_A2_T2");

    [Fact(DisplayName = "prototype/S15.7.3.1_A3")]
    public Task prototype_S15_7_3_1_A3()
        => ExecutionTestFromFile("prototype/S15.7.3.1_A3");

    [Fact(DisplayName = "prototype/S15.7.4_A3.2")]
    public Task prototype_S15_7_4_A3_2()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.2");

    [Fact(DisplayName = "prototype/S15.7.4_A3.3")]
    public Task prototype_S15_7_4_A3_3()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.3");

    [Fact(DisplayName = "prototype/S15.7.4_A3.4")]
    public Task prototype_S15_7_4_A3_4()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.4");

    [Fact(DisplayName = "prototype/S15.7.4_A3.5")]
    public Task prototype_S15_7_4_A3_5()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.5");

    [Fact(DisplayName = "prototype/S15.7.4_A3.6")]
    public Task prototype_S15_7_4_A3_6()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.6");

    [Fact(DisplayName = "prototype/S15.7.4_A3.7")]
    public Task prototype_S15_7_4_A3_7()
        => ExecutionTestFromFile("prototype/S15.7.4_A3.7");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A1_T02")]
    public Task prototype_toString_S15_7_4_2_A1_T02()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A1_T02");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A1_T03")]
    public Task prototype_toString_S15_7_4_2_A1_T03()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A1_T03");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T01")]
    public Task prototype_toString_S15_7_4_2_A2_T01()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T01");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T02")]
    public Task prototype_toString_S15_7_4_2_A2_T02()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T02");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T03")]
    public Task prototype_toString_S15_7_4_2_A2_T03()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T03");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T04")]
    public Task prototype_toString_S15_7_4_2_A2_T04()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T04");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T05")]
    public Task prototype_toString_S15_7_4_2_A2_T05()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T05");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T06")]
    public Task prototype_toString_S15_7_4_2_A2_T06()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T06");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T07")]
    public Task prototype_toString_S15_7_4_2_A2_T07()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T07");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T08")]
    public Task prototype_toString_S15_7_4_2_A2_T08()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T08");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T09")]
    public Task prototype_toString_S15_7_4_2_A2_T09()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T09");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T10")]
    public Task prototype_toString_S15_7_4_2_A2_T10()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T10");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T11")]
    public Task prototype_toString_S15_7_4_2_A2_T11()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T11");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T12")]
    public Task prototype_toString_S15_7_4_2_A2_T12()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T12");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T13")]
    public Task prototype_toString_S15_7_4_2_A2_T13()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T13");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T14")]
    public Task prototype_toString_S15_7_4_2_A2_T14()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T14");

    [Fact(DisplayName = "prototype/toString/S15.7.4.2_A2_T15")]
    public Task prototype_toString_S15_7_4_2_A2_T15()
        => ExecutionTestFromFile("prototype/toString/S15.7.4.2_A2_T15");

    [Fact(DisplayName = "isFinite/arg-is-not-number")]
    public Task isFinite_arg_is_not_number()
        => ExecutionTestFromFile("isFinite/arg-is-not-number");

    [Fact(DisplayName = "isFinite/finite-numbers")]
    public Task isFinite_finite_numbers()
        => ExecutionTestFromFile("isFinite/finite-numbers");

}
