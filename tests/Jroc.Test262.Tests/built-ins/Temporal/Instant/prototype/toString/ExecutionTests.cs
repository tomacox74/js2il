using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.toString;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.toString") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov17_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov17_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov17_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "fractionalseconddigits-auto.js")]
    public Task Port300Nov17_fractionalseconddigits_auto() => ExecutionTestFromFile("fractionalseconddigits-auto");

    [Fact(DisplayName = "fractionalseconddigits-invalid-string.js")]
    public Task Port300Nov17_fractionalseconddigits_invalid_string() => ExecutionTestFromFile("fractionalseconddigits-invalid-string");

    [Fact(DisplayName = "fractionalseconddigits-nan.js")]
    public Task Port300Nov17_fractionalseconddigits_nan() => ExecutionTestFromFile("fractionalseconddigits-nan");

    [Fact(DisplayName = "fractionalseconddigits-negative.js")]
    public Task Port300Nov17_fractionalseconddigits_negative() => ExecutionTestFromFile("fractionalseconddigits-negative");

    [Fact(DisplayName = "fractionalseconddigits-non-integer.js")]
    public Task Port300Nov17_fractionalseconddigits_non_integer() => ExecutionTestFromFile("fractionalseconddigits-non-integer");

    [Fact(DisplayName = "fractionalseconddigits-number.js")]
    public Task Port300Nov17_fractionalseconddigits_number() => ExecutionTestFromFile("fractionalseconddigits-number");

    [Fact(DisplayName = "fractionalseconddigits-out-of-range.js")]
    public Task Port300Nov17_fractionalseconddigits_out_of_range() => ExecutionTestFromFile("fractionalseconddigits-out-of-range");

    [Fact(DisplayName = "fractionalseconddigits-undefined.js")]
    public Task Port300Nov17_fractionalseconddigits_undefined() => ExecutionTestFromFile("fractionalseconddigits-undefined");

    [Fact(DisplayName = "fractionalseconddigits-wrong-type.js")]
    public Task Port300Nov17_fractionalseconddigits_wrong_type() => ExecutionTestFromFile("fractionalseconddigits-wrong-type");

    [Fact(DisplayName = "get-timezone-throws.js")]
    public Task Port300Nov17_get_timezone_throws() => ExecutionTestFromFile("get-timezone-throws");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov17_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov17_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-epochnanoseconds.js")]
    public Task Port300Nov17_negative_epochnanoseconds() => ExecutionTestFromFile("negative-epochnanoseconds");

    [Fact(DisplayName = "negative-instant-rounding.js")]
    public Task Port300Nov17_negative_instant_rounding() => ExecutionTestFromFile("negative-instant-rounding");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov17_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

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

    [Fact(DisplayName = "precision.js")]
    public Task Port300Nov17_precision() => ExecutionTestFromFile("precision");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov17_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "rounding-cross-midnight.js")]
    public Task Port300Nov17_rounding_cross_midnight() => ExecutionTestFromFile("rounding-cross-midnight");

    [Fact(DisplayName = "rounding-direction.js")]
    public Task Port300Nov17_rounding_direction() => ExecutionTestFromFile("rounding-direction");

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

    [Fact(DisplayName = "smallestunit-fractionalseconddigits.js")]
    public Task Port300Nov17_smallestunit_fractionalseconddigits() => ExecutionTestFromFile("smallestunit-fractionalseconddigits");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port300Nov17_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port300Nov17_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port300Nov17_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-valid-units.js")]
    public Task Port300Nov17_smallestunit_valid_units() => ExecutionTestFromFile("smallestunit-valid-units");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port300Nov17_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "timezone-offset.js")]
    public Task Port300Nov17_timezone_offset() => ExecutionTestFromFile("timezone-offset");

    [Fact(DisplayName = "timezone-string-datetime.js")]
    public Task Port300Nov17_timezone_string_datetime() => ExecutionTestFromFile("timezone-string-datetime");

    [Fact(DisplayName = "timezone-string-leap-second.js")]
    public Task Port300Nov17_timezone_string_leap_second() => ExecutionTestFromFile("timezone-string-leap-second");

    [Fact(DisplayName = "timezone-string-multiple-offsets.js")]
    public Task Port300Nov17_timezone_string_multiple_offsets() => ExecutionTestFromFile("timezone-string-multiple-offsets");

    [Fact(DisplayName = "timezone-string-sub-minute-offset.js")]
    public Task Port300Nov17_timezone_string_sub_minute_offset() => ExecutionTestFromFile("timezone-string-sub-minute-offset");

    [Fact(DisplayName = "timezone-string-year-zero.js")]
    public Task Port300Nov17_timezone_string_year_zero() => ExecutionTestFromFile("timezone-string-year-zero");

    [Fact(DisplayName = "timezone-string.js")]
    public Task Port300Nov17_timezone_string() => ExecutionTestFromFile("timezone-string");

    [Fact(DisplayName = "timezone-wrong-type.js")]
    public Task Port300Nov17_timezone_wrong_type() => ExecutionTestFromFile("timezone-wrong-type");

    [Fact(DisplayName = "year-format.js")]
    public Task Port300Nov17_year_format() => ExecutionTestFromFile("year-format");
}
