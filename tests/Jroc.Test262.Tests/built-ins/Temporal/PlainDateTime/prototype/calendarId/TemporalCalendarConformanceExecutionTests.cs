using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime.prototype.calendarId;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDateTime.prototype.calendarId") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");
}
