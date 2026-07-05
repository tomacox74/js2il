using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_33;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_33") { }

    [Fact(DisplayName = "without-utc-offset")]
    public Task without_utc_offset()
        => ExecutionTestFromFile("without-utc-offset");

    [Fact(DisplayName = "year-zero")]
    public Task year_zero()
        => ExecutionTestFromFile("year-zero");

}