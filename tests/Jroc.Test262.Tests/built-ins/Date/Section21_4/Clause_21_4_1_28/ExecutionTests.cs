using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_28;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_28") { }

    [Fact(DisplayName = "S15.9.3.1_A1_T3")]
    public Task S15_9_3_1_A1_T3()
        => ExecutionTestFromFile("S15.9.3.1_A1_T3");

    [Fact(DisplayName = "S15.9.3.1_A1_T4")]
    public Task S15_9_3_1_A1_T4()
        => ExecutionTestFromFile("S15.9.3.1_A1_T4");

}