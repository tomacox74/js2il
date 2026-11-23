using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime.from;

public sealed class TemporalCalendarFollowupExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarFollowupExecutionTests() : base("built_ins.Temporal.PlainDateTime.from") { }

    [Fact(DisplayName = "argument-number.js")]
    public Task Port_argument_number() => ExecutionTestFromFile("argument-number");

    [Fact(DisplayName = "argument-object-month.js")]
    public Task Port_argument_object_month() => ExecutionTestFromFile("argument-object-month");

    [Fact(DisplayName = "argument-object.js")]
    public Task Port_argument_object() => ExecutionTestFromFile("argument-object");

    [Fact(DisplayName = "argument-plaindatetime.js")]
    public Task Port_argument_plaindatetime() => ExecutionTestFromFile("argument-plaindatetime");

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

    [Fact(DisplayName = "argument-string-calendar-case-insensitive.js")]
    public Task Port_argument_string_calendar_case_insensitive() => ExecutionTestFromFile("argument-string-calendar-case-insensitive");

    [Fact(DisplayName = "argument-string-calendar-invalid-iso-string.js")]
    public Task Port_argument_string_calendar_invalid_iso_string() => ExecutionTestFromFile("argument-string-calendar-invalid-iso-string");

    [Fact(DisplayName = "argument-string-comma-decimal-separator.js")]
    public Task Port_argument_string_comma_decimal_separator() => ExecutionTestFromFile("argument-string-comma-decimal-separator");

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

    [Fact(DisplayName = "argument-string-offset.js")]
    public Task Port_argument_string_offset() => ExecutionTestFromFile("argument-string-offset");

    [Fact(DisplayName = "argument-string-optional-data.js")]
    public Task Port_argument_string_optional_data() => ExecutionTestFromFile("argument-string-optional-data");

    [Fact(DisplayName = "argument-string-out-of-range.js")]
    public Task Port_argument_string_out_of_range() => ExecutionTestFromFile("argument-string-out-of-range");

    [Fact(DisplayName = "argument-string-subsecond.js")]
    public Task Port_argument_string_subsecond() => ExecutionTestFromFile("argument-string-subsecond");

    [Fact(DisplayName = "argument-string-time-separators.js")]
    public Task Port_argument_string_time_separators() => ExecutionTestFromFile("argument-string-time-separators");

    [Fact(DisplayName = "argument-string-time-zone-annotation.js")]
    public Task Port_argument_string_time_zone_annotation() => ExecutionTestFromFile("argument-string-time-zone-annotation");

    [Fact(DisplayName = "argument-string-timezone.js")]
    public Task Port_argument_string_timezone() => ExecutionTestFromFile("argument-string-timezone");

    [Fact(DisplayName = "argument-string-too-many-decimals.js")]
    public Task Port_argument_string_too_many_decimals() => ExecutionTestFromFile("argument-string-too-many-decimals");

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string-with-utc-designator.js")]
    public Task Port_argument_string_with_utc_designator() => ExecutionTestFromFile("argument-string-with-utc-designator");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "argument-zoneddatetime-balance-negative-time-units.js")]
    public Task Port_argument_zoneddatetime_balance_negative_time_units() => ExecutionTestFromFile("argument-zoneddatetime-balance-negative-time-units");

    [Fact(DisplayName = "argument-zoneddatetime-negative-epochnanoseconds.js")]
    public Task Port_argument_zoneddatetime_negative_epochnanoseconds() => ExecutionTestFromFile("argument-zoneddatetime-negative-epochnanoseconds");

    [Fact(DisplayName = "builtin.js")]
    public Task Port_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendar-temporal-object.js")]
    public Task Port_calendar_temporal_object() => ExecutionTestFromFile("calendar-temporal-object");

    [Fact(DisplayName = "calendarresolvefields-error-ordering.js")]
    public Task Port_calendarresolvefields_error_ordering() => ExecutionTestFromFile("calendarresolvefields-error-ordering");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "limits.js")]
    public Task Port_limits() => ExecutionTestFromFile("limits");

    [Fact(DisplayName = "month-code-wrong-type.js")]
    public Task Port_month_code_wrong_type() => ExecutionTestFromFile("month-code-wrong-type");

    [Fact(DisplayName = "monthcode-invalid.js")]
    public Task Port_monthcode_invalid() => ExecutionTestFromFile("monthcode-invalid");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-month-or-day.js")]
    public Task Port_negative_month_or_day() => ExecutionTestFromFile("negative-month-or-day");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "observable-get-overflow-argument-primitive.js")]
    public Task Port_observable_get_overflow_argument_primitive() => ExecutionTestFromFile("observable-get-overflow-argument-primitive");

    [Fact(DisplayName = "observable-get-overflow-argument-string-invalid.js")]
    public Task Port_observable_get_overflow_argument_string_invalid() => ExecutionTestFromFile("observable-get-overflow-argument-string-invalid");

    [Fact(DisplayName = "options-object.js")]
    public Task Port_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "overflow-default-constrain.js")]
    public Task Port_overflow_default_constrain() => ExecutionTestFromFile("overflow-default-constrain");

    [Fact(DisplayName = "overflow-invalid-string.js")]
    public Task Port_overflow_invalid_string() => ExecutionTestFromFile("overflow-invalid-string");

    [Fact(DisplayName = "overflow-reject.js")]
    public Task Port_overflow_reject() => ExecutionTestFromFile("overflow-reject");

    [Fact(DisplayName = "overflow-undefined.js")]
    public Task Port_overflow_undefined() => ExecutionTestFromFile("overflow-undefined");

    [Fact(DisplayName = "overflow-wrong-type.js")]
    public Task Port_overflow_wrong_type() => ExecutionTestFromFile("overflow-wrong-type");

    [Fact(DisplayName = "parser.js")]
    public Task Port_parser() => ExecutionTestFromFile("parser");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "roundtrip-from-property-bag.js")]
    public Task Port_roundtrip_from_property_bag() => ExecutionTestFromFile("roundtrip-from-property-bag");

    [Fact(DisplayName = "roundtrip-from-string.js")]
    public Task Port_roundtrip_from_string() => ExecutionTestFromFile("roundtrip-from-string");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port_year_zero() => ExecutionTestFromFile("year-zero");
}
