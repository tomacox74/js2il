using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.length;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.length") { }

    [Fact(DisplayName = "15.4.5.1-3.d-1.js")]
    public Task _15_4_5_1_3_d_1() => ExecutionTestFromFile("15.4.5.1-3.d-1");

    [Fact(DisplayName = "15.4.5.1-3.d-2.js")]
    public Task _15_4_5_1_3_d_2() => ExecutionTestFromFile("15.4.5.1-3.d-2");

    [Fact(DisplayName = "15.4.5.1-3.d-3.js")]
    public Task _15_4_5_1_3_d_3() => ExecutionTestFromFile("15.4.5.1-3.d-3");

    [Fact(DisplayName = "S15.4.2.2_A1.1_T1.js")]
    public Task S15_4_2_2_A1_1_T1() => ExecutionTestFromFile("S15.4.2.2_A1.1_T1");

    [Fact(DisplayName = "S15.4.2.2_A1.1_T2.js")]
    public Task S15_4_2_2_A1_1_T2() => ExecutionTestFromFile("S15.4.2.2_A1.1_T2");

    [Fact(DisplayName = "S15.4.2.2_A1.1_T3.js")]
    public Task S15_4_2_2_A1_1_T3() => ExecutionTestFromFile("S15.4.2.2_A1.1_T3");

    [Fact(DisplayName = "S15.4.2.2_A1.2_T1.js")]
    public Task S15_4_2_2_A1_2_T1() => ExecutionTestFromFile("S15.4.2.2_A1.2_T1");

    [Fact(DisplayName = "S15.4.2.2_A2.2_T1.js")]
    public Task S15_4_2_2_A2_2_T1() => ExecutionTestFromFile("S15.4.2.2_A2.2_T1");

    [Fact(DisplayName = "S15.4.2.2_A2.2_T2.js")]
    public Task S15_4_2_2_A2_2_T2() => ExecutionTestFromFile("S15.4.2.2_A2.2_T2");

    [Fact(DisplayName = "S15.4.2.2_A2.2_T3.js")]
    public Task S15_4_2_2_A2_2_T3() => ExecutionTestFromFile("S15.4.2.2_A2.2_T3");

    [Fact(DisplayName = "S15.4.2.2_A2.3_T1.js")]
    public Task S15_4_2_2_A2_3_T1() => ExecutionTestFromFile("S15.4.2.2_A2.3_T1");

    [Fact(DisplayName = "S15.4.2.2_A2.3_T2.js")]
    public Task S15_4_2_2_A2_3_T2() => ExecutionTestFromFile("S15.4.2.2_A2.3_T2");

    [Fact(DisplayName = "S15.4.2.2_A2.3_T3.js")]
    public Task S15_4_2_2_A2_3_T3() => ExecutionTestFromFile("S15.4.2.2_A2.3_T3");

    [Fact(DisplayName = "S15.4.2.2_A2.3_T4.js")]
    public Task S15_4_2_2_A2_3_T4() => ExecutionTestFromFile("S15.4.2.2_A2.3_T4");

    [Fact(DisplayName = "S15.4.2.2_A2.3_T5.js")]
    public Task S15_4_2_2_A2_3_T5() => ExecutionTestFromFile("S15.4.2.2_A2.3_T5");

    [Fact(DisplayName = "S15.4.5.1_A1.1_T1.js")]
    public Task S15_4_5_1_A1_1_T1() => ExecutionTestFromFile("S15.4.5.1_A1.1_T1");

    [Fact(DisplayName = "S15.4.5.1_A1.1_T2.js")]
    public Task S15_4_5_1_A1_1_T2() => ExecutionTestFromFile("S15.4.5.1_A1.1_T2");

    [Fact(DisplayName = "S15.4.5.1_A1.2_T1.js")]
    public Task S15_4_5_1_A1_2_T1() => ExecutionTestFromFile("S15.4.5.1_A1.2_T1");

    [Fact(DisplayName = "S15.4.5.1_A1.2_T3.js")]
    public Task S15_4_5_1_A1_2_T3() => ExecutionTestFromFile("S15.4.5.1_A1.2_T3");

    [Fact(DisplayName = "S15.4.5.1_A1.3_T1.js")]
    public Task S15_4_5_1_A1_3_T1() => ExecutionTestFromFile("S15.4.5.1_A1.3_T1");

    [Fact(DisplayName = "S15.4.5.1_A1.3_T2.js")]
    public Task S15_4_5_1_A1_3_T2() => ExecutionTestFromFile("S15.4.5.1_A1.3_T2");

    [Fact(DisplayName = "S15.4.5.2_A3_T4.js")]
    public Task S15_4_5_2_A3_T4() => ExecutionTestFromFile("S15.4.5.2_A3_T4");

    [Fact(DisplayName = "define-own-prop-length-error.js")]
    public Task define_own_prop_length_error() => ExecutionTestFromFile("define-own-prop-length-error");

}
