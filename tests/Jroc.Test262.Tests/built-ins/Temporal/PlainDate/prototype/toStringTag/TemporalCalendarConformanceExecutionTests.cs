using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.toStringTag;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.toStringTag") { }

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");
}
