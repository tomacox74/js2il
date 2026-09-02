using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array") { }

    [Fact(DisplayName = "S15.4.1_A1.1_T2.js")]
    public Task S15_4_1_A1_1_T2() => ExecutionTestFromFile("S15.4.1_A1.1_T2");

    [Fact(DisplayName = "S15.4.1_A1.2_T1.js")]
    public Task S15_4_1_A1_2_T1() => ExecutionTestFromFile("S15.4.1_A1.2_T1");

    [Fact(DisplayName = "S15.4.2.1_A1.1_T2.js")]
    public Task S15_4_2_1_A1_1_T2() => ExecutionTestFromFile("S15.4.2.1_A1.1_T2");

    [Fact(DisplayName = "S15.4.2.1_A1.2_T1.js")]
    public Task S15_4_2_1_A1_2_T1() => ExecutionTestFromFile("S15.4.2.1_A1.2_T1");

    [Fact(DisplayName = "S15.4.2.1_A1.3_T1.js")]
    public Task S15_4_2_1_A1_3_T1() => ExecutionTestFromFile("S15.4.2.1_A1.3_T1");

    [Fact(DisplayName = "S15.4.2.1_A2.2_T1.js")]
    public Task S15_4_2_1_A2_2_T1() => ExecutionTestFromFile("S15.4.2.1_A2.2_T1");

    [Fact(DisplayName = "S15.4.3_A1.1_T1.js")]
    public Task S15_4_3_A1_1_T1() => ExecutionTestFromFile("S15.4.3_A1.1_T1");

    [Fact(DisplayName = "S15.4.3_A1.1_T2.js")]
    public Task S15_4_3_A1_1_T2() => ExecutionTestFromFile("S15.4.3_A1.1_T2");

    [Fact(DisplayName = "S15.4.3_A1.1_T3.js")]
    public Task S15_4_3_A1_1_T3() => ExecutionTestFromFile("S15.4.3_A1.1_T3");

    [Fact(DisplayName = "S15.4.5.1_A1.2_T2.js")]
    public Task S15_4_5_1_A1_2_T2() => ExecutionTestFromFile("S15.4.5.1_A1.2_T2");

    [Fact(DisplayName = "S15.4.5.2_A3_T3.js")]
    public Task S15_4_5_2_A3_T3() => ExecutionTestFromFile("S15.4.5.2_A3_T3");

    [Fact(DisplayName = "S15.4_A1.1_T10.js")]
    public Task S15_4_A1_1_T10() => ExecutionTestFromFile("S15.4_A1.1_T10");

    [Fact(DisplayName = "S15.4_A1.1_T4.js")]
    public Task S15_4_A1_1_T4() => ExecutionTestFromFile("S15.4_A1.1_T4");

    [Fact(DisplayName = "S15.4_A1.1_T5.js")]
    public Task S15_4_A1_1_T5() => ExecutionTestFromFile("S15.4_A1.1_T5");

    [Fact(DisplayName = "S15.4_A1.1_T6.js")]
    public Task S15_4_A1_1_T6() => ExecutionTestFromFile("S15.4_A1.1_T6");

    [Fact(DisplayName = "S15.4_A1.1_T7.js")]
    public Task S15_4_A1_1_T7() => ExecutionTestFromFile("S15.4_A1.1_T7");

    [Fact(DisplayName = "S15.4_A1.1_T8.js")]
    public Task S15_4_A1_1_T8() => ExecutionTestFromFile("S15.4_A1.1_T8");

    [Fact(DisplayName = "S15.4_A1.1_T9.js")]
    public Task S15_4_A1_1_T9() => ExecutionTestFromFile("S15.4_A1.1_T9");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

}
