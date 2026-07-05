using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_32_1;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_32_1") { }

    [Fact(DisplayName = "year-zero")]
    public Task year_zero()
        => ExecutionTestFromFile("year-zero");

    [Fact(DisplayName = "year-zero_2")]
    public Task year_zero_2()
        => ExecutionTestFromFile("year-zero_2");

}