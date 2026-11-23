using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime.prototype.monthCode;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainDateTime.prototype.monthCode") { }

    [Fact(DisplayName = "no-leap-months.js")]
    public Task Port_no_leap_months() => ExecutionTestFromFile("no-leap-months");
}
