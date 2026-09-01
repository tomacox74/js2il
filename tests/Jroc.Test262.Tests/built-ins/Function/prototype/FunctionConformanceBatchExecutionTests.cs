using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype") { }

    [Fact(DisplayName = "S15.3.3.1_A2.js")]
    public Task S15_3_3_1_A2() => ExecutionTestFromFile("S15.3.3.1_A2");

    [Fact(DisplayName = "S15.3.3.1_A4.js")]
    public Task S15_3_3_1_A4() => ExecutionTestFromFile("S15.3.3.1_A4");

    [Fact(DisplayName = "S15.3.4_A1.js")]
    public Task S15_3_4_A1() => ExecutionTestFromFile("S15.3.4_A1");

    [Fact(DisplayName = "S15.3.4_A2_T1.js")]
    public Task S15_3_4_A2_T1() => ExecutionTestFromFile("S15.3.4_A2_T1");

    [Fact(DisplayName = "S15.3.4_A2_T2.js")]
    public Task S15_3_4_A2_T2() => ExecutionTestFromFile("S15.3.4_A2_T2");

    [Fact(DisplayName = "S15.3.4_A2_T3.js")]
    public Task S15_3_4_A2_T3() => ExecutionTestFromFile("S15.3.4_A2_T3");

    [Fact(DisplayName = "S15.3.4_A3_T1.js")]
    public Task S15_3_4_A3_T1() => ExecutionTestFromFile("S15.3.4_A3_T1");

    [Fact(DisplayName = "S15.3.4_A3_T2.js")]
    public Task S15_3_4_A3_T2() => ExecutionTestFromFile("S15.3.4_A3_T2");

    [Fact(DisplayName = "S15.3.4_A4.js")]
    public Task S15_3_4_A4() => ExecutionTestFromFile("S15.3.4_A4");

    [Fact(DisplayName = "S15.3.4_A5.js")]
    public Task S15_3_4_A5() => ExecutionTestFromFile("S15.3.4_A5");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

}
