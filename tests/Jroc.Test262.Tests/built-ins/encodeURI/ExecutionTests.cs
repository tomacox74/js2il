using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.encodeURI;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.encodeURI") { }

    [Fact(DisplayName = "S15.1.3.3_A1.1_T1")]
    public Task S15_1_3_3_A1_1_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.1_T1");

    [Fact(DisplayName = "S15.1.3.3_A1.1_T2")]
    public Task S15_1_3_3_A1_1_T2()
        => ExecutionTestFromFile("S15.1.3.3_A1.1_T2");

    [Fact(DisplayName = "S15.1.3.3_A1.2_T1")]
    public Task S15_1_3_3_A1_2_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.2_T1");

    [Fact(DisplayName = "S15.1.3.3_A1.2_T2")]
    public Task S15_1_3_3_A1_2_T2()
        => ExecutionTestFromFile("S15.1.3.3_A1.2_T2");

    [Fact(DisplayName = "S15.1.3.3_A1.3_T1")]
    public Task S15_1_3_3_A1_3_T1()
        => ExecutionTestFromFile("S15.1.3.3_A1.3_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.1_T1")]
    public Task S15_1_3_3_A2_1_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.1_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.2_T1")]
    public Task S15_1_3_3_A2_2_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.2_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.3_T1")]
    public Task S15_1_3_3_A2_3_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.3_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.4_T1")]
    public Task S15_1_3_3_A2_4_T1()
        => ExecutionTestFromFile("S15.1.3.3_A2.4_T1");

    [Fact(DisplayName = "S15.1.3.3_A2.4_T2")]
    public Task S15_1_3_3_A2_4_T2()
        => ExecutionTestFromFile("S15.1.3.3_A2.4_T2");
}
