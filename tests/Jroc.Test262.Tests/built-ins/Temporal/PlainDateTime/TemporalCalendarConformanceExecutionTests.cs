using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDateTime") { }

    [Fact(DisplayName = "builtin.js")]
    public Task Port_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendar-case-insensitive.js")]
    public Task Port_calendar_case_insensitive() => ExecutionTestFromFile("calendar-case-insensitive");

    [Fact(DisplayName = "calendar-invalid-iso-string.js")]
    public Task Port_calendar_invalid_iso_string() => ExecutionTestFromFile("calendar-invalid-iso-string");

    [Fact(DisplayName = "calendar-string.js")]
    public Task Port_calendar_string() => ExecutionTestFromFile("calendar-string");

    [Fact(DisplayName = "calendar-undefined.js")]
    public Task Port_calendar_undefined() => ExecutionTestFromFile("calendar-undefined");

    [Fact(DisplayName = "calendar-wrong-type.js")]
    public Task Port_calendar_wrong_type() => ExecutionTestFromFile("calendar-wrong-type");

    [Fact(DisplayName = "constructor.js")]
    public Task Port_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "get-prototype-from-constructor-throws.js")]
    public Task Port_get_prototype_from_constructor_throws() => ExecutionTestFromFile("get-prototype-from-constructor-throws");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "limits.js")]
    public Task Port_limits() => ExecutionTestFromFile("limits");

    [Fact(DisplayName = "missing-arguments.js")]
    public Task Port_missing_arguments() => ExecutionTestFromFile("missing-arguments");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "throws-if-date-is-invalid.js")]
    public Task Port_throws_if_date_is_invalid() => ExecutionTestFromFile("throws-if-date-is-invalid");

    [Fact(DisplayName = "throws-if-time-is-invalid.js")]
    public Task Port_throws_if_time_is_invalid() => ExecutionTestFromFile("throws-if-time-is-invalid");
}
