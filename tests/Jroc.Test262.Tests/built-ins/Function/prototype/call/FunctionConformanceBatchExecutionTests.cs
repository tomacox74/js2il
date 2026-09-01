using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.call;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.call") { }

    [Fact(DisplayName = "S15.3.4.4_A3_T2.js")]
    public Task S15_3_4_4_A3_T2() => ExecutionTestFromFile("S15.3.4.4_A3_T2");

    [Fact(DisplayName = "S15.3.4.4_A3_T3.js")]
    public Task S15_3_4_4_A3_T3() => ExecutionTestFromFile("S15.3.4.4_A3_T3");

    [Fact(DisplayName = "S15.3.4.4_A3_T4.js")]
    public Task S15_3_4_4_A3_T4() => ExecutionTestFromFile("S15.3.4.4_A3_T4");

    [Fact(DisplayName = "S15.3.4.4_A3_T5.js")]
    public Task S15_3_4_4_A3_T5() => ExecutionTestFromFile("S15.3.4.4_A3_T5");

    [Fact(DisplayName = "S15.3.4.4_A3_T6.js")]
    public Task S15_3_4_4_A3_T6() => ExecutionTestFromFile("S15.3.4.4_A3_T6");

    [Fact(DisplayName = "S15.3.4.4_A3_T7.js")]
    public Task S15_3_4_4_A3_T7() => ExecutionTestFromFile("S15.3.4.4_A3_T7");

    [Fact(DisplayName = "S15.3.4.4_A3_T8.js")]
    public Task S15_3_4_4_A3_T8() => ExecutionTestFromFile("S15.3.4.4_A3_T8");

    [Fact(DisplayName = "S15.3.4.4_A5_T4.js")]
    public Task S15_3_4_4_A5_T4() => ExecutionTestFromFile("S15.3.4.4_A5_T4");

    [Fact(DisplayName = "S15.3.4.4_A5_T5.js")]
    public Task S15_3_4_4_A5_T5() => ExecutionTestFromFile("S15.3.4.4_A5_T5");

    [Fact(DisplayName = "S15.3.4.4_A5_T6.js")]
    public Task S15_3_4_4_A5_T6() => ExecutionTestFromFile("S15.3.4.4_A5_T6");

    [Fact(DisplayName = "S15.3.4.4_A5_T7.js")]
    public Task S15_3_4_4_A5_T7() => ExecutionTestFromFile("S15.3.4.4_A5_T7");

    [Fact(DisplayName = "S15.3.4.4_A5_T8.js")]
    public Task S15_3_4_4_A5_T8() => ExecutionTestFromFile("S15.3.4.4_A5_T8");

    [Fact(DisplayName = "S15.3.4.4_A6_T1.js")]
    public Task S15_3_4_4_A6_T1() => ExecutionTestFromFile("S15.3.4.4_A6_T1");

    [Fact(DisplayName = "S15.3.4.4_A6_T10.js")]
    public Task S15_3_4_4_A6_T10() => ExecutionTestFromFile("S15.3.4.4_A6_T10");

    [Fact(DisplayName = "S15.3.4.4_A6_T2.js")]
    public Task S15_3_4_4_A6_T2() => ExecutionTestFromFile("S15.3.4.4_A6_T2");

    [Fact(DisplayName = "S15.3.4.4_A6_T5.js")]
    public Task S15_3_4_4_A6_T5() => ExecutionTestFromFile("S15.3.4.4_A6_T5");

    [Fact(DisplayName = "S15.3.4.4_A6_T6.js")]
    public Task S15_3_4_4_A6_T6() => ExecutionTestFromFile("S15.3.4.4_A6_T6");

    [Fact(DisplayName = "S15.3.4.4_A6_T7.js")]
    public Task S15_3_4_4_A6_T7() => ExecutionTestFromFile("S15.3.4.4_A6_T7");

    [Fact(DisplayName = "S15.3.4.4_A6_T8.js")]
    public Task S15_3_4_4_A6_T8() => ExecutionTestFromFile("S15.3.4.4_A6_T8");

    [Fact(DisplayName = "S15.3.4.4_A6_T9.js")]
    public Task S15_3_4_4_A6_T9() => ExecutionTestFromFile("S15.3.4.4_A6_T9");

    [Fact(DisplayName = "S15.3.4.4_A7_T3.js")]
    public Task S15_3_4_4_A7_T3() => ExecutionTestFromFile("S15.3.4.4_A7_T3");

    [Fact(DisplayName = "S15.3.4.4_A7_T4.js")]
    public Task S15_3_4_4_A7_T4() => ExecutionTestFromFile("S15.3.4.4_A7_T4");

    [Fact(DisplayName = "S15.3.4.4_A7_T5.js")]
    public Task S15_3_4_4_A7_T5() => ExecutionTestFromFile("S15.3.4.4_A7_T5");

    [Fact(DisplayName = "S15.3.4.4_A7_T6.js")]
    public Task S15_3_4_4_A7_T6() => ExecutionTestFromFile("S15.3.4.4_A7_T6");

    [Fact(DisplayName = "S15.3.4.4_A9.js")]
    public Task S15_3_4_4_A9() => ExecutionTestFromFile("S15.3.4.4_A9");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
