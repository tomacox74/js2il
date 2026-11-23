using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.compare;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDate.compare") { }

    [Fact(DisplayName = "argument-zoneddatetime-slots.js")]
    public Task Port_argument_zoneddatetime_slots() => ExecutionTestFromFile("argument-zoneddatetime-slots");

    [Fact(DisplayName = "argument-zoneddatetime.js")]
    public Task Port_argument_zoneddatetime() => ExecutionTestFromFile("argument-zoneddatetime");
}
