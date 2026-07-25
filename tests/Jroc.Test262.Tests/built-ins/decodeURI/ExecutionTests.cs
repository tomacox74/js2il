using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.decodeURI;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.decodeURI") { }

    [Fact(DisplayName = "S15.1.3.1_A1.1_T1")]
    public Task S15_1_3_1_A1_1_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.1_T1");

    [Fact(DisplayName = "S15.1.3.1_A1.2_T1")]
    public Task S15_1_3_1_A1_2_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.2_T1");

    [Fact(DisplayName = "S15.1.3.1_A1.2_T2")]
    public Task S15_1_3_1_A1_2_T2()
        => ExecutionTestFromFile("S15.1.3.1_A1.2_T2");

    [Fact(DisplayName = "S15.1.3.1_A1.3_T1")]
    public Task S15_1_3_1_A1_3_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.3_T1");

    [Fact(DisplayName = "S15.1.3.1_A1.4_T1")]
    public Task S15_1_3_1_A1_4_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.4_T1");

    [Fact(DisplayName = "S15.1.3.1_A1.5_T1")]
    public Task S15_1_3_1_A1_5_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.5_T1");

    [Fact(DisplayName = "S15.1.3.1_A1.6_T1")]
    public Task S15_1_3_1_A1_6_T1()
        => ExecutionTestFromFile("S15.1.3.1_A1.6_T1");

    [Fact(DisplayName = "S15.1.3.1_A2.2_T1")]
    public Task S15_1_3_1_A2_2_T1()
        => ExecutionTestFromFile("S15.1.3.1_A2.2_T1");

    [Fact(DisplayName = "S15.1.3.1_A2.3_T1")]
    public Task S15_1_3_1_A2_3_T1()
        => ExecutionTestFromFile("S15.1.3.1_A2.3_T1");

    [Fact(DisplayName = "S15.1.3.1_A3_T1")]
    public Task S15_1_3_1_A3_T1()
        => ExecutionTestFromFile("S15.1.3.1_A3_T1");
}
