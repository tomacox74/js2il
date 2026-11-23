using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainDateTime") { }

    [Fact(DisplayName = "argument-convert.js")]
    public Task Port_argument_convert() => ExecutionTestFromFile("argument-convert");

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "constructor-full.js")]
    public Task Port_constructor_full() => ExecutionTestFromFile("constructor-full");

    [Fact(DisplayName = "hour-undefined.js")]
    public Task Port_hour_undefined() => ExecutionTestFromFile("hour-undefined");

    [Fact(DisplayName = "microsecond-undefined.js")]
    public Task Port_microsecond_undefined() => ExecutionTestFromFile("microsecond-undefined");

    [Fact(DisplayName = "millisecond-undefined.js")]
    public Task Port_millisecond_undefined() => ExecutionTestFromFile("millisecond-undefined");

    [Fact(DisplayName = "minute-undefined.js")]
    public Task Port_minute_undefined() => ExecutionTestFromFile("minute-undefined");

    [Fact(DisplayName = "nanosecond-undefined.js")]
    public Task Port_nanosecond_undefined() => ExecutionTestFromFile("nanosecond-undefined");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "second-undefined.js")]
    public Task Port_second_undefined() => ExecutionTestFromFile("second-undefined");

    [Fact(DisplayName = "subclass.js")]
    public Task Port_subclass() => ExecutionTestFromFile("subclass");
}
