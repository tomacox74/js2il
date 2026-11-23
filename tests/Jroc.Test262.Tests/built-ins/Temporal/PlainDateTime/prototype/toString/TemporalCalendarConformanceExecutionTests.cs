using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDateTime.prototype.toString;

public sealed class TemporalCalendarConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TemporalCalendarConformanceExecutionTests() : base("built_ins.Temporal.PlainDateTime.prototype.toString") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port_basic() => ExecutionTestFromFile("basic");

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

    [Fact(DisplayName = "fractionalseconddigits-auto.js")]
    public Task Port_fractionalseconddigits_auto() => ExecutionTestFromFile("fractionalseconddigits-auto");

    [Fact(DisplayName = "fractionalseconddigits-invalid-string.js")]
    public Task Port_fractionalseconddigits_invalid_string() => ExecutionTestFromFile("fractionalseconddigits-invalid-string");

    [Fact(DisplayName = "fractionalseconddigits-nan.js")]
    public Task Port_fractionalseconddigits_nan() => ExecutionTestFromFile("fractionalseconddigits-nan");

    [Fact(DisplayName = "fractionalseconddigits-negative.js")]
    public Task Port_fractionalseconddigits_negative() => ExecutionTestFromFile("fractionalseconddigits-negative");

    [Fact(DisplayName = "fractionalseconddigits-non-integer.js")]
    public Task Port_fractionalseconddigits_non_integer() => ExecutionTestFromFile("fractionalseconddigits-non-integer");

    [Fact(DisplayName = "fractionalseconddigits-number.js")]
    public Task Port_fractionalseconddigits_number() => ExecutionTestFromFile("fractionalseconddigits-number");

    [Fact(DisplayName = "fractionalseconddigits-out-of-range.js")]
    public Task Port_fractionalseconddigits_out_of_range() => ExecutionTestFromFile("fractionalseconddigits-out-of-range");

    [Fact(DisplayName = "fractionalseconddigits-undefined.js")]
    public Task Port_fractionalseconddigits_undefined() => ExecutionTestFromFile("fractionalseconddigits-undefined");

    [Fact(DisplayName = "fractionalseconddigits-wrong-type.js")]
    public Task Port_fractionalseconddigits_wrong_type() => ExecutionTestFromFile("fractionalseconddigits-wrong-type");

    [Fact(DisplayName = "length.js")]
    public Task Port_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-object.js")]
    public Task Port_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "rounding-cross-midnight.js")]
    public Task Port_rounding_cross_midnight() => ExecutionTestFromFile("rounding-cross-midnight");

    [Fact(DisplayName = "rounding-direction.js")]
    public Task Port_rounding_direction() => ExecutionTestFromFile("rounding-direction");

    [Fact(DisplayName = "rounding-edge-of-range.js")]
    public Task Port_rounding_edge_of_range() => ExecutionTestFromFile("rounding-edge-of-range");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "smallestunit-fractionalseconddigits.js")]
    public Task Port_smallestunit_fractionalseconddigits() => ExecutionTestFromFile("smallestunit-fractionalseconddigits");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-valid-units.js")]
    public Task Port_smallestunit_valid_units() => ExecutionTestFromFile("smallestunit-valid-units");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "year-format.js")]
    public Task Port_year_format() => ExecutionTestFromFile("year-format");
}
