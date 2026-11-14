using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.compare;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.compare") { }

    [Fact(DisplayName = "argument-cast.js")]
    public Task Port1000_argument_cast() => ExecutionTestFromFile("argument-cast");

    [Fact(DisplayName = "argument-number.js")]
    public Task Port1000_argument_number() => ExecutionTestFromFile("argument-number");

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

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port1000_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string-with-time-designator.js")]
    public Task Port1000_argument_string_with_time_designator() => ExecutionTestFromFile("argument-string-with-time-designator");

    [Fact(DisplayName = "argument-string-with-utc-designator.js")]
    public Task Port1000_argument_string_with_utc_designator() => ExecutionTestFromFile("argument-string-with-utc-designator");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port1000_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "basic.js")]
    public Task Port1000_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port1000_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "exhaustive.js")]
    public Task Port1000_exhaustive() => ExecutionTestFromFile("exhaustive");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port1000_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port1000_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port1000_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port1000_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port1000_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "plaintime-propertybag-no-time-units.js")]
    public Task Port1000_plaintime_propertybag_no_time_units() => ExecutionTestFromFile("plaintime-propertybag-no-time-units");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port1000_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "use-internal-slots.js")]
    public Task Port1000_use_internal_slots() => ExecutionTestFromFile("use-internal-slots");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port1000_year_zero() => ExecutionTestFromFile("year-zero");

}
