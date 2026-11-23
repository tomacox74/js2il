using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.equals;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.equals") { }

    [Fact(DisplayName = "argument-zoneddatetime-slots.js")]
    public Task Port_argument_zoneddatetime_slots() => ExecutionTestFromFile("argument-zoneddatetime-slots");
}
