using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_32;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_32") { }

    [Fact(DisplayName = "15.9.1.15-1")]
    public Task _15_9_1_15_1()
        => ExecutionTestFromFile("15.9.1.15-1");

    [Fact(DisplayName = "year-zero")]
    public Task year_zero()
        => ExecutionTestFromFile("year-zero");

}