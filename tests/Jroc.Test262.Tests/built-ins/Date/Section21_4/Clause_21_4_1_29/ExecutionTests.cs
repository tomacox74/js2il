using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_29;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_29") { }

    [Fact(DisplayName = "S15.9.3.1_A1_T5")]
    public Task S15_9_3_1_A1_T5()
        => ExecutionTestFromFile("S15.9.3.1_A1_T5");

    [Fact(DisplayName = "S15.9.3.1_A1_T6")]
    public Task S15_9_3_1_A1_T6()
        => ExecutionTestFromFile("S15.9.3.1_A1_T6");

}