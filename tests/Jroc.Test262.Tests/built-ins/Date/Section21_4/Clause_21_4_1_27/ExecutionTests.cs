using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_27;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_27") { }

    [Fact(DisplayName = "S15.9.3.1_A1_T1")]
    public Task S15_9_3_1_A1_T1()
        => ExecutionTestFromFile("S15.9.3.1_A1_T1");

    [Fact(DisplayName = "S15.9.3.1_A1_T2")]
    public Task S15_9_3_1_A1_T2()
        => ExecutionTestFromFile("S15.9.3.1_A1_T2");

}