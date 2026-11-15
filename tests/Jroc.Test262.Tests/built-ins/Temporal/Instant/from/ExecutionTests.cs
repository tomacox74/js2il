using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.from;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.from") { }

    [Fact(DisplayName = "argument-instant.js")]
    public Task Port300InstantNov15_argument_instant() => ExecutionTestFromFile("argument-instant");

    [Fact(DisplayName = "argument-object-tostring.js")]
    public Task Port300InstantNov15_argument_object_tostring() => ExecutionTestFromFile("argument-object-tostring");

    [Fact(DisplayName = "argument-string-calendar-annotation-invalid-key.js")]
    public Task Port300InstantNov15_argument_string_calendar_annotation_invalid_key() => ExecutionTestFromFile("argument-string-calendar-annotation-invalid-key");

    [Fact(DisplayName = "argument-string-calendar-annotation.js")]
    public Task Port300InstantNov15_argument_string_calendar_annotation() => ExecutionTestFromFile("argument-string-calendar-annotation");

    [Fact(DisplayName = "argument-string-critical-unknown-annotation.js")]
    public Task Port300InstantNov15_argument_string_critical_unknown_annotation() => ExecutionTestFromFile("argument-string-critical-unknown-annotation");

    [Fact(DisplayName = "argument-string-invalid.js")]
    public Task Port300InstantNov15_argument_string_invalid() => ExecutionTestFromFile("argument-string-invalid");

    [Fact(DisplayName = "argument-string-limits.js")]
    public Task Port300InstantNov15_argument_string_limits() => ExecutionTestFromFile("argument-string-limits");

    [Fact(DisplayName = "argument-string-minus-sign.js")]
    public Task Port300InstantNov15_argument_string_minus_sign() => ExecutionTestFromFile("argument-string-minus-sign");

    [Fact(DisplayName = "argument-string-multiple-calendar.js")]
    public Task Port300InstantNov15_argument_string_multiple_calendar() => ExecutionTestFromFile("argument-string-multiple-calendar");

    [Fact(DisplayName = "argument-string-multiple-time-zone.js")]
    public Task Port300InstantNov15_argument_string_multiple_time_zone() => ExecutionTestFromFile("argument-string-multiple-time-zone");

    [Fact(DisplayName = "argument-string-time-separators.js")]
    public Task Port300InstantNov15_argument_string_time_separators() => ExecutionTestFromFile("argument-string-time-separators");

    [Fact(DisplayName = "argument-string-too-many-decimals.js")]
    public Task Port300InstantNov15_argument_string_too_many_decimals() => ExecutionTestFromFile("argument-string-too-many-decimals");

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port300InstantNov15_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port300InstantNov15_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port300InstantNov15_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "basic.js")]
    public Task Port300InstantNov15_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "instant-string-multiple-offsets.js")]
    public Task Port300InstantNov15_instant_string_multiple_offsets() => ExecutionTestFromFile("instant-string-multiple-offsets");

    [Fact(DisplayName = "instant-string-sub-minute-offset.js")]
    public Task Port300InstantNov15_instant_string_sub_minute_offset() => ExecutionTestFromFile("instant-string-sub-minute-offset");

    [Fact(DisplayName = "instant-string.js")]
    public Task Port300InstantNov15_instant_string() => ExecutionTestFromFile("instant-string");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port300InstantNov15_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port300InstantNov15_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300InstantNov15_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300InstantNov15_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

    [Fact(DisplayName = "timezone-custom.js")]
    public Task Port300InstantNov15_timezone_custom() => ExecutionTestFromFile("timezone-custom");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port300InstantNov15_year_zero() => ExecutionTestFromFile("year-zero");
}
