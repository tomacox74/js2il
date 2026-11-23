using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainMonthDay.prototype.calendarId;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainMonthDay.prototype.calendarId") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");
}
