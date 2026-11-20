using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.since;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.since") { }

    [Fact(DisplayName = "add-subtract.js")]
    public Task Port300InstantNov15_add_subtract() => ExecutionTestFromFile("add-subtract");

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

    [Fact(DisplayName = "argument-wrong-type.js")]
    public Task Port300InstantNov15_argument_wrong_type() => ExecutionTestFromFile("argument-wrong-type");

    [Fact(DisplayName = "blank-result.js")]
    public Task Port300InstantNov15_blank_result() => ExecutionTestFromFile("blank-result");

    [Fact(DisplayName = "branding.js")]
    public Task Port300InstantNov15_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "float64-representable-integer.js")]
    public Task Port300InstantNov15_float64_representable_integer() => ExecutionTestFromFile("float64-representable-integer");

    [Fact(DisplayName = "instant-string-multiple-offsets.js")]
    public Task Port300InstantNov15_instant_string_multiple_offsets() => ExecutionTestFromFile("instant-string-multiple-offsets");

    [Fact(DisplayName = "instant-string-sub-minute-offset.js")]
    public Task Port300InstantNov15_instant_string_sub_minute_offset() => ExecutionTestFromFile("instant-string-sub-minute-offset");

    [Fact(DisplayName = "instant-string.js")]
    public Task Port300InstantNov15_instant_string() => ExecutionTestFromFile("instant-string");

    [Fact(DisplayName = "invalid-increments.js")]
    public Task Port300InstantNov15_invalid_increments() => ExecutionTestFromFile("invalid-increments");

    [Fact(DisplayName = "largest-unit-default.js")]
    public Task Port300InstantNov15_largest_unit_default() => ExecutionTestFromFile("largest-unit-default");

    [Fact(DisplayName = "largestunit-invalid-string.js")]
    public Task Port300InstantNov15_largestunit_invalid_string() => ExecutionTestFromFile("largestunit-invalid-string");

    [Fact(DisplayName = "largestunit-plurals-accepted.js")]
    public Task Port300InstantNov15_largestunit_plurals_accepted() => ExecutionTestFromFile("largestunit-plurals-accepted");

    [Fact(DisplayName = "largestunit-smallestunit-mismatch.js")]
    public Task Port300InstantNov15_largestunit_smallestunit_mismatch() => ExecutionTestFromFile("largestunit-smallestunit-mismatch");

    [Fact(DisplayName = "largestunit-undefined.js")]
    public Task Port300InstantNov15_largestunit_undefined() => ExecutionTestFromFile("largestunit-undefined");

    [Fact(DisplayName = "largestunit-wrong-type.js")]
    public Task Port300InstantNov15_largestunit_wrong_type() => ExecutionTestFromFile("largestunit-wrong-type");

    [Fact(DisplayName = "largestunit.js")]
    public Task Port300InstantNov15_largestunit() => ExecutionTestFromFile("largestunit");

    [Fact(DisplayName = "leap-second.js")]
    public Task Port300InstantNov15_leap_second() => ExecutionTestFromFile("leap-second");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "minutes-and-hours.js")]
    public Task Port300InstantNov15_minutes_and_hours() => ExecutionTestFromFile("minutes-and-hours");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "no-fractional-minutes-hours.js")]
    public Task Port300InstantNov15_no_fractional_minutes_hours() => ExecutionTestFromFile("no-fractional-minutes-hours");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300InstantNov15_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-may-be-function.js")]
    public Task Port300InstantNov15_options_may_be_function() => ExecutionTestFromFile("options-may-be-function");

    [Fact(DisplayName = "options-object.js")]
    public Task Port300InstantNov15_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port300InstantNov15_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port300InstantNov15_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port300InstantNov15_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300InstantNov15_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "round-cross-unit-boundary.js")]
    public Task Port300InstantNov15_round_cross_unit_boundary() => ExecutionTestFromFile("round-cross-unit-boundary");

    [Fact(DisplayName = "rounding-increments.js")]
    public Task Port300InstantNov15_rounding_increments() => ExecutionTestFromFile("rounding-increments");

    [Fact(DisplayName = "roundingincrement-nan.js")]
    public Task Port300InstantNov15_roundingincrement_nan() => ExecutionTestFromFile("roundingincrement-nan");

    [Fact(DisplayName = "roundingincrement-non-integer.js")]
    public Task Port300InstantNov15_roundingincrement_non_integer() => ExecutionTestFromFile("roundingincrement-non-integer");

    [Fact(DisplayName = "roundingincrement-out-of-range.js")]
    public Task Port300InstantNov15_roundingincrement_out_of_range() => ExecutionTestFromFile("roundingincrement-out-of-range");

    [Fact(DisplayName = "roundingincrement-undefined.js")]
    public Task Port300InstantNov15_roundingincrement_undefined() => ExecutionTestFromFile("roundingincrement-undefined");

    [Fact(DisplayName = "roundingincrement-wrong-type.js")]
    public Task Port300InstantNov15_roundingincrement_wrong_type() => ExecutionTestFromFile("roundingincrement-wrong-type");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port300InstantNov15_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port300InstantNov15_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port300InstantNov15_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port300InstantNov15_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port300InstantNov15_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port300InstantNov15_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port300InstantNov15_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port300InstantNov15_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port300InstantNov15_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port300InstantNov15_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port300InstantNov15_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port300InstantNov15_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port300InstantNov15_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port300InstantNov15_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port300InstantNov15_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port300InstantNov15_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "subseconds.js")]
    public Task Port300InstantNov15_subseconds() => ExecutionTestFromFile("subseconds");

    [Fact(DisplayName = "valid-increments.js")]
    public Task Port300InstantNov15_valid_increments() => ExecutionTestFromFile("valid-increments");

    [Fact(DisplayName = "year-zero.js")]
    public Task Port300InstantNov15_year_zero() => ExecutionTestFromFile("year-zero");

    [Fact(DisplayName = "argument-string-date-with-utc-offset.js")]
    public Task Port300Nov17_argument_string_date_with_utc_offset() => ExecutionTestFromFile("argument-string-date-with-utc-offset");

    [Fact(DisplayName = "argument-string-time-zone-annotation.js")]
    public Task Port300Nov17_argument_string_time_zone_annotation() => ExecutionTestFromFile("argument-string-time-zone-annotation");
}
