using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_31;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_31") { }

    [Fact(DisplayName = "TimeClip_negative_zero")]
    public Task TimeClip_negative_zero()
        => ExecutionTestFromFile("TimeClip_negative_zero");

    [Fact(DisplayName = "S9.4_A3_T1")]
    public Task S9_4_A3_T1()
        => ExecutionTestFromFile("S9.4_A3_T1");

}