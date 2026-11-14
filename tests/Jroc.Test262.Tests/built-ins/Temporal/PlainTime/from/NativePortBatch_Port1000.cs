using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.from;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.from") { }

    [Fact(DisplayName = "argument-number.js")]
    public Task Port1000_argument_number() => ExecutionTestFromFile("argument-number");

    [Fact(DisplayName = "argument-object-leap-second.js")]
    public Task Port1000_argument_object_leap_second() => ExecutionTestFromFile("argument-object-leap-second");

    [Fact(DisplayName = "argument-object.js")]
    public Task Port1000_argument_object() => ExecutionTestFromFile("argument-object");

    [Fact(DisplayName = "argument-plaintime.js")]
    public Task Port1000_argument_plaintime() => ExecutionTestFromFile("argument-plaintime");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port1000_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-string-calendar-annotation-invalid-key.js")]
    public Task Port1000_argument_string_calendar_annotation_invalid_key() => ExecutionTestFromFile("argument-string-calendar-annotation-invalid-key");

    [Fact(DisplayName = "argument-string-calendar-annotation.js")]
    public Task Port1000_argument_string_calendar_annotation() => ExecutionTestFromFile("argument-string-calendar-annotation");

    [Fact(DisplayName = "argument-string-critical-unknown-annotation.js")]
    public Task Port1000_argument_string_critical_unknown_annotation() => ExecutionTestFromFile("argument-string-critical-unknown-annotation");

    [Fact(DisplayName = "argument-string-date-with-utc-offset.js")]
    public Task Port1000_argument_string_date_with_utc_offset() => ExecutionTestFromFile("argument-string-date-with-utc-offset");

    [Fact(DisplayName = "argument-string-invalid.js")]
    public Task Port1000_argument_string_invalid() => ExecutionTestFromFile("argument-string-invalid");

    [Fact(DisplayName = "argument-string-leap-second.js")]
    public Task Port1000_argument_string_leap_second() => ExecutionTestFromFile("argument-string-leap-second");

    [Fact(DisplayName = "argument-string-minus-sign.js")]
    public Task Port1000_argument_string_minus_sign() => ExecutionTestFromFile("argument-string-minus-sign");

    [Fact(DisplayName = "argument-string-multiple-calendar.js")]
    public Task Port1000_argument_string_multiple_calendar() => ExecutionTestFromFile("argument-string-multiple-calendar");

    [Fact(DisplayName = "argument-string-multiple-time-zone.js")]
    public Task Port1000_argument_string_multiple_time_zone() => ExecutionTestFromFile("argument-string-multiple-time-zone");

    [Fact(DisplayName = "argument-string-no-implicit-midnight.js")]
    public Task Port1000_argument_string_no_implicit_midnight() => ExecutionTestFromFile("argument-string-no-implicit-midnight");

    [Fact(DisplayName = "argument-string-time-designator-required-for-disambiguation.js")]
    public Task Port1000_argument_string_time_designator_required_for_disambiguation() => ExecutionTestFromFile("argument-string-time-designator-required-for-disambiguation");

    [Fact(DisplayName = "argument-string-time-separators.js")]
    public Task Port1000_argument_string_time_separators() => ExecutionTestFromFile("argument-string-time-separators");

    [Fact(DisplayName = "argument-string-time-zone-annotation.js")]
    public Task Port1000_argument_string_time_zone_annotation() => ExecutionTestFromFile("argument-string-time-zone-annotation");

    [Fact(DisplayName = "argument-string-too-many-decimals.js")]
    public Task Port1000_argument_string_too_many_decimals() => ExecutionTestFromFile("argument-string-too-many-decimals");

    [Fact(DisplayName = "argument-string-trailing-junk.js")]
    public Task Port1000_argument_string_trailing_junk() => ExecutionTestFromFile("argument-string-trailing-junk");

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port1000_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string-with-time-designator.js")]
    public Task Port1000_argument_string_with_time_designator() => ExecutionTestFromFile("argument-string-with-time-designator");

    [Fact(DisplayName = "argument-string-with-utc-designator.js")]
    public Task Port1000_argument_string_with_utc_designator() => ExecutionTestFromFile("argument-string-with-utc-designator");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port1000_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port1000_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port1000_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port1000_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port1000_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "observable-get-overflow-argument-string-invalid.js")]
    public Task Port1000_observable_get_overflow_argument_string_invalid() => ExecutionTestFromFile("observable-get-overflow-argument-string-invalid");

    [Fact(DisplayName = "options-invalid.js")]
    public Task Port1000_options_invalid() => ExecutionTestFromFile("options-invalid");

    [Fact(DisplayName = "options-object.js")]
    public Task Port1000_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port1000_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port1000_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "overflow-constrain.js")]
    public Task Port1000_overflow_constrain() => ExecutionTestFromFile("overflow-constrain");

    [Fact(DisplayName = "overflow-invalid-string.js")]
    public Task Port1000_overflow_invalid_string() => ExecutionTestFromFile("overflow-invalid-string");

    [Fact(DisplayName = "overflow-reject.js")]
    public Task Port1000_overflow_reject() => ExecutionTestFromFile("overflow-reject");

    [Fact(DisplayName = "overflow-undefined.js")]
    public Task Port1000_overflow_undefined() => ExecutionTestFromFile("overflow-undefined");

    [Fact(DisplayName = "overflow-wrong-type.js")]
    public Task Port1000_overflow_wrong_type() => ExecutionTestFromFile("overflow-wrong-type");

    [Fact(DisplayName = "plaintime-propertybag-no-time-units.js")]
    public Task Port1000_plaintime_propertybag_no_time_units() => ExecutionTestFromFile("plaintime-propertybag-no-time-units");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port1000_year_zero() => ExecutionTestFromFile("year-zero");

}
