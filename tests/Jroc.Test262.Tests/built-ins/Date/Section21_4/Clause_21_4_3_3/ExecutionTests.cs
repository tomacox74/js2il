using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_3_3;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_3_3") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.9.5_A01_T1")]
    public Task S15_9_5_A01_T1()
        => ExecutionTestFromFile("S15.9.5_A01_T1");

}