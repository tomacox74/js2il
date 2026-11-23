using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime.compare;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainDateTime.compare") { }

    [Fact(DisplayName = "argument-number.js")]
    public Task Port_argument_number() => ExecutionTestFromFile("argument-number");

    [Fact(DisplayName = "argument-object-insufficient-data.js")]
    public Task Port_argument_object_insufficient_data() => ExecutionTestFromFile("argument-object-insufficient-data");

    [Fact(DisplayName = "argument-propertybag-calendar-case-insensitive.js")]
    public Task Port_argument_propertybag_calendar_case_insensitive() => ExecutionTestFromFile("argument-propertybag-calendar-case-insensitive");

    [Fact(DisplayName = "argument-propertybag-calendar-invalid-iso-string.js")]
    public Task Port_argument_propertybag_calendar_invalid_iso_string() => ExecutionTestFromFile("argument-propertybag-calendar-invalid-iso-string");

    [Fact(DisplayName = "argument-propertybag-calendar-iso-string.js")]
    public Task Port_argument_propertybag_calendar_iso_string() => ExecutionTestFromFile("argument-propertybag-calendar-iso-string");

    [Fact(DisplayName = "argument-propertybag-calendar-leap-second.js")]
    public Task Port_argument_propertybag_calendar_leap_second() => ExecutionTestFromFile("argument-propertybag-calendar-leap-second");

    [Fact(DisplayName = "argument-propertybag-calendar-string.js")]
    public Task Port_argument_propertybag_calendar_string() => ExecutionTestFromFile("argument-propertybag-calendar-string");

    [Fact(DisplayName = "argument-propertybag-calendar-wrong-type.js")]
    public Task Port_argument_propertybag_calendar_wrong_type() => ExecutionTestFromFile("argument-propertybag-calendar-wrong-type");

    [Fact(DisplayName = "argument-propertybag-calendar-year-zero.js")]
    public Task Port_argument_propertybag_calendar_year_zero() => ExecutionTestFromFile("argument-propertybag-calendar-year-zero");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-string-calendar-annotation-invalid-key.js")]
    public Task Port_argument_string_calendar_annotation_invalid_key() => ExecutionTestFromFile("argument-string-calendar-annotation-invalid-key");

    [Fact(DisplayName = "argument-string-calendar-annotation.js")]
    public Task Port_argument_string_calendar_annotation() => ExecutionTestFromFile("argument-string-calendar-annotation");

    [Fact(DisplayName = "argument-string-calendar-invalid-iso-string.js")]
    public Task Port_argument_string_calendar_invalid_iso_string() => ExecutionTestFromFile("argument-string-calendar-invalid-iso-string");

    [Fact(DisplayName = "argument-string-critical-unknown-annotation.js")]
    public Task Port_argument_string_critical_unknown_annotation() => ExecutionTestFromFile("argument-string-critical-unknown-annotation");

    [Fact(DisplayName = "argument-string-date-with-utc-offset.js")]
    public Task Port_argument_string_date_with_utc_offset() => ExecutionTestFromFile("argument-string-date-with-utc-offset");

    [Fact(DisplayName = "argument-string-invalid.js")]
    public Task Port_argument_string_invalid() => ExecutionTestFromFile("argument-string-invalid");

    [Fact(DisplayName = "argument-string-limits.js")]
    public Task Port_argument_string_limits() => ExecutionTestFromFile("argument-string-limits");

    [Fact(DisplayName = "argument-string-minus-sign.js")]
    public Task Port_argument_string_minus_sign() => ExecutionTestFromFile("argument-string-minus-sign");

    [Fact(DisplayName = "argument-string-multiple-calendar.js")]
    public Task Port_argument_string_multiple_calendar() => ExecutionTestFromFile("argument-string-multiple-calendar");

    [Fact(DisplayName = "argument-string-multiple-time-zone.js")]
    public Task Port_argument_string_multiple_time_zone() => ExecutionTestFromFile("argument-string-multiple-time-zone");

    [Fact(DisplayName = "argument-string-time-separators.js")]
    public Task Port_argument_string_time_separators() => ExecutionTestFromFile("argument-string-time-separators");

    [Fact(DisplayName = "argument-string-time-zone-annotation.js")]
    public Task Port_argument_string_time_zone_annotation() => ExecutionTestFromFile("argument-string-time-zone-annotation");

    [Fact(DisplayName = "argument-string-too-many-decimals.js")]
    public Task Port_argument_string_too_many_decimals() => ExecutionTestFromFile("argument-string-too-many-decimals");

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string-with-utc-designator.js")]
    public Task Port_argument_string_with_utc_designator() => ExecutionTestFromFile("argument-string-with-utc-designator");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "argument-zoneddatetime-negative-epochnanoseconds.js")]
    public Task Port_argument_zoneddatetime_negative_epochnanoseconds() => ExecutionTestFromFile("argument-zoneddatetime-negative-epochnanoseconds");

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendar-temporal-object.js")]
    public Task Port_calendar_temporal_object() => ExecutionTestFromFile("calendar-temporal-object");

    [Fact(DisplayName = "cast.js")]
    public Task Port_cast() => ExecutionTestFromFile("cast");

    [Fact(DisplayName = "exhaustive.js")]
    public Task Port_exhaustive() => ExecutionTestFromFile("exhaustive");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "use-internal-slots.js")]
    public Task Port_use_internal_slots() => ExecutionTestFromFile("use-internal-slots");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port_year_zero() => ExecutionTestFromFile("year-zero");
}
