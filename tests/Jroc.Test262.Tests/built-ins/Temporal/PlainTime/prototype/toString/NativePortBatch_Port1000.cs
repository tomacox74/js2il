using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.toString;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype.toString") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port1000_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port1000_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "fractionalseconddigits-auto.js")]
    public Task Port1000_fractionalseconddigits_auto() => ExecutionTestFromFile("fractionalseconddigits-auto");

    [Fact(DisplayName = "fractionalseconddigits-invalid-string.js")]
    public Task Port1000_fractionalseconddigits_invalid_string() => ExecutionTestFromFile("fractionalseconddigits-invalid-string");

    [Fact(DisplayName = "fractionalseconddigits-nan.js")]
    public Task Port1000_fractionalseconddigits_nan() => ExecutionTestFromFile("fractionalseconddigits-nan");

    [Fact(DisplayName = "fractionalseconddigits-non-integer.js")]
    public Task Port1000_fractionalseconddigits_non_integer() => ExecutionTestFromFile("fractionalseconddigits-non-integer");

    [Fact(DisplayName = "fractionalseconddigits-number.js")]
    public Task Port1000_fractionalseconddigits_number() => ExecutionTestFromFile("fractionalseconddigits-number");

    [Fact(DisplayName = "fractionalseconddigits-out-of-range.js")]
    public Task Port1000_fractionalseconddigits_out_of_range() => ExecutionTestFromFile("fractionalseconddigits-out-of-range");

    [Fact(DisplayName = "fractionalseconddigits-undefined.js")]
    public Task Port1000_fractionalseconddigits_undefined() => ExecutionTestFromFile("fractionalseconddigits-undefined");

    [Fact(DisplayName = "fractionalseconddigits-wrong-type.js")]
    public Task Port1000_fractionalseconddigits_wrong_type() => ExecutionTestFromFile("fractionalseconddigits-wrong-type");

    [Fact(DisplayName = "options-invalid.js")]
    public Task Port1000_options_invalid() => ExecutionTestFromFile("options-invalid");

    [Fact(DisplayName = "options-object.js")]
    public Task Port1000_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port1000_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port1000_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port1000_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port1000_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "rounding-cross-midnight.js")]
    public Task Port1000_rounding_cross_midnight() => ExecutionTestFromFile("rounding-cross-midnight");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port1000_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port1000_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port1000_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port1000_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port1000_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port1000_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port1000_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port1000_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port1000_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port1000_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port1000_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port1000_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "smallestunit-fractionalseconddigits.js")]
    public Task Port1000_smallestunit_fractionalseconddigits() => ExecutionTestFromFile("smallestunit-fractionalseconddigits");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port1000_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port1000_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port1000_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-valid-units.js")]
    public Task Port1000_smallestunit_valid_units() => ExecutionTestFromFile("smallestunit-valid-units");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port1000_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

}
