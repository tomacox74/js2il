using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainYearMonth.prototype.month;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainYearMonth.prototype.month") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");
}
