using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.Section21_4.Clause_21_4_1_22;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.Section21_4.Clause_21_4_1_22") { }

    [Fact(DisplayName = "without-utc-offset")]
    public Task without_utc_offset()
        => ExecutionTestFromFile("without-utc-offset");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}