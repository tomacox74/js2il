using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.from;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDate.from") { }

    [Fact(DisplayName = "argument-zoneddatetime-slots.js")]
    public Task Port_argument_zoneddatetime_slots() => ExecutionTestFromFile("argument-zoneddatetime-slots");

    [Fact(DisplayName = "argument-zoneddatetime.js")]
    public Task Port_argument_zoneddatetime() => ExecutionTestFromFile("argument-zoneddatetime");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "overflow-invalid-string.js")]
    public Task Port_overflow_invalid_string() => ExecutionTestFromFile("overflow-invalid-string");
}
