using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.apply;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.apply") { }

    [Fact(DisplayName = "15.3.4.3-1-s.js")]
    public Task _15_3_4_3_1_s() => ExecutionTestFromFile("15.3.4.3-1-s");

    [Fact(DisplayName = "15.3.4.3-2-s.js")]
    public Task _15_3_4_3_2_s() => ExecutionTestFromFile("15.3.4.3-2-s");

    [Fact(DisplayName = "15.3.4.3-3-s.js")]
    public Task _15_3_4_3_3_s() => ExecutionTestFromFile("15.3.4.3-3-s");

    [Fact(DisplayName = "S15.3.4.3_A12.js")]
    public Task S15_3_4_3_A12() => ExecutionTestFromFile("S15.3.4.3_A12");

    [Fact(DisplayName = "S15.3.4.3_A1_T1.js")]
    public Task S15_3_4_3_A1_T1() => ExecutionTestFromFile("S15.3.4.3_A1_T1");

    [Fact(DisplayName = "S15.3.4.3_A1_T2.js")]
    public Task S15_3_4_3_A1_T2() => ExecutionTestFromFile("S15.3.4.3_A1_T2");

    [Fact(DisplayName = "S15.3.4.3_A3_T1.js")]
    public Task S15_3_4_3_A3_T1() => ExecutionTestFromFile("S15.3.4.3_A3_T1");

    [Fact(DisplayName = "S15.3.4.3_A3_T2.js")]
    public Task S15_3_4_3_A3_T2() => ExecutionTestFromFile("S15.3.4.3_A3_T2");

    [Fact(DisplayName = "S15.3.4.3_A3_T3.js")]
    public Task S15_3_4_3_A3_T3() => ExecutionTestFromFile("S15.3.4.3_A3_T3");

    [Fact(DisplayName = "S15.3.4.3_A3_T4.js")]
    public Task S15_3_4_3_A3_T4() => ExecutionTestFromFile("S15.3.4.3_A3_T4");

    [Fact(DisplayName = "S15.3.4.3_A3_T5.js")]
    public Task S15_3_4_3_A3_T5() => ExecutionTestFromFile("S15.3.4.3_A3_T5");

    [Fact(DisplayName = "S15.3.4.3_A3_T6.js")]
    public Task S15_3_4_3_A3_T6() => ExecutionTestFromFile("S15.3.4.3_A3_T6");

    [Fact(DisplayName = "S15.3.4.3_A3_T7.js")]
    public Task S15_3_4_3_A3_T7() => ExecutionTestFromFile("S15.3.4.3_A3_T7");

    [Fact(DisplayName = "S15.3.4.3_A3_T8.js")]
    public Task S15_3_4_3_A3_T8() => ExecutionTestFromFile("S15.3.4.3_A3_T8");

    [Fact(DisplayName = "S15.3.4.3_A5_T4.js")]
    public Task S15_3_4_3_A5_T4() => ExecutionTestFromFile("S15.3.4.3_A5_T4");

    [Fact(DisplayName = "S15.3.4.3_A5_T5.js")]
    public Task S15_3_4_3_A5_T5() => ExecutionTestFromFile("S15.3.4.3_A5_T5");

    [Fact(DisplayName = "S15.3.4.3_A5_T6.js")]
    public Task S15_3_4_3_A5_T6() => ExecutionTestFromFile("S15.3.4.3_A5_T6");

    [Fact(DisplayName = "S15.3.4.3_A5_T7.js")]
    public Task S15_3_4_3_A5_T7() => ExecutionTestFromFile("S15.3.4.3_A5_T7");

    [Fact(DisplayName = "S15.3.4.3_A5_T8.js")]
    public Task S15_3_4_3_A5_T8() => ExecutionTestFromFile("S15.3.4.3_A5_T8");

    [Fact(DisplayName = "S15.3.4.3_A7_T1.js")]
    public Task S15_3_4_3_A7_T1() => ExecutionTestFromFile("S15.3.4.3_A7_T1");

    [Fact(DisplayName = "S15.3.4.3_A7_T10.js")]
    public Task S15_3_4_3_A7_T10() => ExecutionTestFromFile("S15.3.4.3_A7_T10");

    [Fact(DisplayName = "S15.3.4.3_A7_T2.js")]
    public Task S15_3_4_3_A7_T2() => ExecutionTestFromFile("S15.3.4.3_A7_T2");

    [Fact(DisplayName = "S15.3.4.3_A7_T5.js")]
    public Task S15_3_4_3_A7_T5() => ExecutionTestFromFile("S15.3.4.3_A7_T5");

    [Fact(DisplayName = "S15.3.4.3_A7_T6.js")]
    public Task S15_3_4_3_A7_T6() => ExecutionTestFromFile("S15.3.4.3_A7_T6");

    [Fact(DisplayName = "S15.3.4.3_A7_T7.js")]
    public Task S15_3_4_3_A7_T7() => ExecutionTestFromFile("S15.3.4.3_A7_T7");

    [Fact(DisplayName = "S15.3.4.3_A7_T8.js")]
    public Task S15_3_4_3_A7_T8() => ExecutionTestFromFile("S15.3.4.3_A7_T8");

    [Fact(DisplayName = "S15.3.4.3_A7_T9.js")]
    public Task S15_3_4_3_A7_T9() => ExecutionTestFromFile("S15.3.4.3_A7_T9");

    [Fact(DisplayName = "S15.3.4.3_A8_T3.js")]
    public Task S15_3_4_3_A8_T3() => ExecutionTestFromFile("S15.3.4.3_A8_T3");

    [Fact(DisplayName = "S15.3.4.3_A8_T4.js")]
    public Task S15_3_4_3_A8_T4() => ExecutionTestFromFile("S15.3.4.3_A8_T4");

    [Fact(DisplayName = "S15.3.4.3_A8_T5.js")]
    public Task S15_3_4_3_A8_T5() => ExecutionTestFromFile("S15.3.4.3_A8_T5");

    [Fact(DisplayName = "S15.3.4.3_A8_T6.js")]
    public Task S15_3_4_3_A8_T6() => ExecutionTestFromFile("S15.3.4.3_A8_T6");

    [Fact(DisplayName = "argarray-not-object.js")]
    public Task argarray_not_object() => ExecutionTestFromFile("argarray-not-object");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "this-not-callable.js")]
    public Task this_not_callable() => ExecutionTestFromFile("this-not-callable");

}
