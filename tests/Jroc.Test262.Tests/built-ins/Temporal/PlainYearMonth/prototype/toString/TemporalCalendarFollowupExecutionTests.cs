using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainYearMonth.prototype.toString;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainYearMonth.prototype.toString") { }

    [Fact(DisplayName = "branding.js")]
    public Task Port_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendarname-always.js")]
    public Task Port_calendarname_always() => ExecutionTestFromFile("calendarname-always");

    [Fact(DisplayName = "calendarname-auto.js")]
    public Task Port_calendarname_auto() => ExecutionTestFromFile("calendarname-auto");

    [Fact(DisplayName = "calendarname-critical.js")]
    public Task Port_calendarname_critical() => ExecutionTestFromFile("calendarname-critical");

    [Fact(DisplayName = "calendarname-invalid-string.js")]
    public Task Port_calendarname_invalid_string() => ExecutionTestFromFile("calendarname-invalid-string");

    [Fact(DisplayName = "calendarname-never.js")]
    public Task Port_calendarname_never() => ExecutionTestFromFile("calendarname-never");

    [Fact(DisplayName = "calendarname-undefined.js")]
    public Task Port_calendarname_undefined() => ExecutionTestFromFile("calendarname-undefined");

    [Fact(DisplayName = "calendarname-wrong-type.js")]
    public Task Port_calendarname_wrong_type() => ExecutionTestFromFile("calendarname-wrong-type");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-object.js")]
    public Task Port_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "year-format.js")]
    public Task Port_year_format() => ExecutionTestFromFile("year-format");
}
