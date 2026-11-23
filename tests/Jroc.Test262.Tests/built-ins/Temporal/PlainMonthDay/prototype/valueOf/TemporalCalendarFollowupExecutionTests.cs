using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainMonthDay.prototype.valueOf;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainMonthDay.prototype.valueOf") { }

    [Fact(DisplayName = "branding.js")]
    public Task Port_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");
}
