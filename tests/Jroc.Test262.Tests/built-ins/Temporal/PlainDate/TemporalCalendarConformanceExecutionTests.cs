using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDate") { }

    [Fact(DisplayName = "subclass.js")]
    public Task Port_subclass() => ExecutionTestFromFile("subclass");
}
