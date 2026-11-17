using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.until;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainTime.prototype.until") { }

    [Fact(DisplayName = "argument-cast.js")]
    public Task Port300Nov17_argument_cast() => ExecutionTestFromFile("argument-cast");

    [Fact(DisplayName = "argument-number.js")]
    public Task Port300Nov17_argument_number() => ExecutionTestFromFile("argument-number");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300Nov17_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-string-calendar-annotation-invalid-key.js")]
    public Task Port300Nov17_argument_string_calendar_annotation_invalid_key() => ExecutionTestFromFile("argument-string-calendar-annotation-invalid-key");

    [Fact(DisplayName = "argument-string-calendar-annotation.js")]
    public Task Port300Nov17_argument_string_calendar_annotation() => ExecutionTestFromFile("argument-string-calendar-annotation");

    [Fact(DisplayName = "argument-string-critical-unknown-annotation.js")]
    public Task Port300Nov17_argument_string_critical_unknown_annotation() => ExecutionTestFromFile("argument-string-critical-unknown-annotation");

    [Fact(DisplayName = "argument-string-date-with-utc-offset.js")]
    public Task Port300Nov17_argument_string_date_with_utc_offset() => ExecutionTestFromFile("argument-string-date-with-utc-offset");

    [Fact(DisplayName = "argument-string-minus-sign.js")]
    public Task Port300Nov17_argument_string_minus_sign() => ExecutionTestFromFile("argument-string-minus-sign");

    [Fact(DisplayName = "argument-string-multiple-calendar.js")]
    public Task Port300Nov17_argument_string_multiple_calendar() => ExecutionTestFromFile("argument-string-multiple-calendar");

    [Fact(DisplayName = "argument-string-multiple-time-zone.js")]
    public Task Port300Nov17_argument_string_multiple_time_zone() => ExecutionTestFromFile("argument-string-multiple-time-zone");

    [Fact(DisplayName = "argument-string-no-implicit-midnight.js")]
    public Task Port300Nov17_argument_string_no_implicit_midnight() => ExecutionTestFromFile("argument-string-no-implicit-midnight");

    [Fact(DisplayName = "argument-string-time-designator-required-for-disambiguation.js")]
    public Task Port300Nov17_argument_string_time_designator_required_for_disambiguation() => ExecutionTestFromFile("argument-string-time-designator-required-for-disambiguation");

    [Fact(DisplayName = "argument-string-time-separators.js")]
    public Task Port300Nov17_argument_string_time_separators() => ExecutionTestFromFile("argument-string-time-separators");

    [Fact(DisplayName = "argument-string-time-zone-annotation.js")]
    public Task Port300Nov17_argument_string_time_zone_annotation() => ExecutionTestFromFile("argument-string-time-zone-annotation");

    [Fact(DisplayName = "argument-string-too-many-decimals.js")]
    public Task Port300Nov17_argument_string_too_many_decimals() => ExecutionTestFromFile("argument-string-too-many-decimals");

    [Fact(DisplayName = "argument-string-unknown-annotation.js")]
    public Task Port300Nov17_argument_string_unknown_annotation() => ExecutionTestFromFile("argument-string-unknown-annotation");

    [Fact(DisplayName = "argument-string-with-time-designator.js")]
    public Task Port300Nov17_argument_string_with_time_designator() => ExecutionTestFromFile("argument-string-with-time-designator");

    [Fact(DisplayName = "argument-string-with-utc-designator.js")]
    public Task Port300Nov17_argument_string_with_utc_designator() => ExecutionTestFromFile("argument-string-with-utc-designator");

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port300Nov17_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "balance-negative-time-units.js")]
    public Task Port300Nov17_balance_negative_time_units() => ExecutionTestFromFile("balance-negative-time-units");

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov17_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "blank-result.js")]
    public Task Port300Nov17_blank_result() => ExecutionTestFromFile("blank-result");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov17_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov17_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "largestunit-invalid-string.js")]
    public Task Port300Nov17_largestunit_invalid_string() => ExecutionTestFromFile("largestunit-invalid-string");

    [Fact(DisplayName = "largestunit-plurals-accepted.js")]
    public Task Port300Nov17_largestunit_plurals_accepted() => ExecutionTestFromFile("largestunit-plurals-accepted");

    [Fact(DisplayName = "largestunit-smallestunit-mismatch.js")]
    public Task Port300Nov17_largestunit_smallestunit_mismatch() => ExecutionTestFromFile("largestunit-smallestunit-mismatch");

    [Fact(DisplayName = "largestunit-undefined.js")]
    public Task Port300Nov17_largestunit_undefined() => ExecutionTestFromFile("largestunit-undefined");

    [Fact(DisplayName = "largestunit-wrong-type.js")]
    public Task Port300Nov17_largestunit_wrong_type() => ExecutionTestFromFile("largestunit-wrong-type");

    [Fact(DisplayName = "largestunit.js")]
    public Task Port300Nov17_largestunit() => ExecutionTestFromFile("largestunit");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port300Nov17_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov17_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov17_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov17_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-invalid.js")]
    public Task Port300Nov17_options_invalid() => ExecutionTestFromFile("options-invalid");

    [Fact(DisplayName = "options-object.js")]
    public Task Port300Nov17_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port300Nov17_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port300Nov17_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port300Nov17_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov17_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "plaintime-propertybag-no-time-units.js")]
    public Task Port300Nov17_plaintime_propertybag_no_time_units() => ExecutionTestFromFile("plaintime-propertybag-no-time-units");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov17_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "result-sub-second.js")]
    public Task Port300Nov17_result_sub_second() => ExecutionTestFromFile("result-sub-second");

    [Fact(DisplayName = "round-cross-unit-boundary.js")]
    public Task Port300Nov17_round_cross_unit_boundary() => ExecutionTestFromFile("round-cross-unit-boundary");

    [Fact(DisplayName = "roundingincrement-hours.js")]
    public Task Port300Nov17_roundingincrement_hours() => ExecutionTestFromFile("roundingincrement-hours");

    [Fact(DisplayName = "roundingincrement-invalid.js")]
    public Task Port300Nov17_roundingincrement_invalid() => ExecutionTestFromFile("roundingincrement-invalid");

    [Fact(DisplayName = "roundingincrement-microseconds.js")]
    public Task Port300Nov17_roundingincrement_microseconds() => ExecutionTestFromFile("roundingincrement-microseconds");

    [Fact(DisplayName = "roundingincrement-milliseconds.js")]
    public Task Port300Nov17_roundingincrement_milliseconds() => ExecutionTestFromFile("roundingincrement-milliseconds");

    [Fact(DisplayName = "roundingincrement-minutes.js")]
    public Task Port300Nov17_roundingincrement_minutes() => ExecutionTestFromFile("roundingincrement-minutes");

    [Fact(DisplayName = "roundingincrement-nan.js")]
    public Task Port300Nov17_roundingincrement_nan() => ExecutionTestFromFile("roundingincrement-nan");

    [Fact(DisplayName = "roundingincrement-nanoseconds.js")]
    public Task Port300Nov17_roundingincrement_nanoseconds() => ExecutionTestFromFile("roundingincrement-nanoseconds");

    [Fact(DisplayName = "roundingincrement-non-integer.js")]
    public Task Port300Nov17_roundingincrement_non_integer() => ExecutionTestFromFile("roundingincrement-non-integer");

    [Fact(DisplayName = "roundingincrement-out-of-range.js")]
    public Task Port300Nov17_roundingincrement_out_of_range() => ExecutionTestFromFile("roundingincrement-out-of-range");

    [Fact(DisplayName = "roundingincrement-seconds.js")]
    public Task Port300Nov17_roundingincrement_seconds() => ExecutionTestFromFile("roundingincrement-seconds");

    [Fact(DisplayName = "roundingincrement-undefined.js")]
    public Task Port300Nov17_roundingincrement_undefined() => ExecutionTestFromFile("roundingincrement-undefined");

    [Fact(DisplayName = "roundingincrement-wrong-type.js")]
    public Task Port300Nov17_roundingincrement_wrong_type() => ExecutionTestFromFile("roundingincrement-wrong-type");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port300Nov17_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port300Nov17_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port300Nov17_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port300Nov17_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port300Nov17_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port300Nov17_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port300Nov17_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port300Nov17_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port300Nov17_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port300Nov17_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port300Nov17_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port300Nov17_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port300Nov17_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port300Nov17_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port300Nov17_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port300Nov17_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port300Nov17_year_zero() => ExecutionTestFromFile("year-zero");
}
