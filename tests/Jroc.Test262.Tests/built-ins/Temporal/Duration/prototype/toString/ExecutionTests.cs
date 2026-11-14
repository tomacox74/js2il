using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.toString;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.toString") { }

    [Fact(DisplayName = "balance-subseconds.js")]
    public Task Port300Nov14_balance_subseconds() => ExecutionTestFromFile("balance-subseconds");

    [Fact(DisplayName = "balance.js")]
    public Task Port300Nov14_balance() => ExecutionTestFromFile("balance");

    [Fact(DisplayName = "blank-duration-precision.js")]
    public Task Port300Nov14_blank_duration_precision() => ExecutionTestFromFile("blank-duration-precision");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "fractionalseconddigits-auto.js")]
    public Task Port300Nov14_fractionalseconddigits_auto() => ExecutionTestFromFile("fractionalseconddigits-auto");

    [Fact(DisplayName = "fractionalseconddigits-exact-number-of-digits.js")]
    public Task Port300Nov14_fractionalseconddigits_exact_number_of_digits() => ExecutionTestFromFile("fractionalseconddigits-exact-number-of-digits");

    [Fact(DisplayName = "fractionalseconddigits-invalid-string.js")]
    public Task Port300Nov14_fractionalseconddigits_invalid_string() => ExecutionTestFromFile("fractionalseconddigits-invalid-string");

    [Fact(DisplayName = "fractionalseconddigits-nan.js")]
    public Task Port300Nov14_fractionalseconddigits_nan() => ExecutionTestFromFile("fractionalseconddigits-nan");

    [Fact(DisplayName = "fractionalseconddigits-non-integer.js")]
    public Task Port300Nov14_fractionalseconddigits_non_integer() => ExecutionTestFromFile("fractionalseconddigits-non-integer");

    [Fact(DisplayName = "fractionalseconddigits-number.js")]
    public Task Port300Nov14_fractionalseconddigits_number() => ExecutionTestFromFile("fractionalseconddigits-number");

    [Fact(DisplayName = "fractionalseconddigits-out-of-range.js")]
    public Task Port300Nov14_fractionalseconddigits_out_of_range() => ExecutionTestFromFile("fractionalseconddigits-out-of-range");

    [Fact(DisplayName = "fractionalseconddigits-undefined.js")]
    public Task Port300Nov14_fractionalseconddigits_undefined() => ExecutionTestFromFile("fractionalseconddigits-undefined");

    [Fact(DisplayName = "fractionalseconddigits-wrong-type.js")]
    public Task Port300Nov14_fractionalseconddigits_wrong_type() => ExecutionTestFromFile("fractionalseconddigits-wrong-type");

    [Fact(DisplayName = "large-with-small-units.js")]
    public Task Port300Nov14_large_with_small_units() => ExecutionTestFromFile("large-with-small-units");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "max-value.js")]
    public Task Port300Nov14_max_value() => ExecutionTestFromFile("max-value");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-components.js")]
    public Task Port300Nov14_negative_components() => ExecutionTestFromFile("negative-components");

    [Fact(DisplayName = "no-precision-loss.js")]
    public Task Port300Nov14_no_precision_loss() => ExecutionTestFromFile("no-precision-loss");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-object.js")]
    public Task Port300Nov14_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port300Nov14_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port300Nov14_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port300Nov14_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov14_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "precision.js")]
    public Task Port300Nov14_precision() => ExecutionTestFromFile("precision");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "round-cross-unit-boundary.js")]
    public Task Port300Nov14_round_cross_unit_boundary() => ExecutionTestFromFile("round-cross-unit-boundary");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port300Nov14_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port300Nov14_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port300Nov14_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port300Nov14_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port300Nov14_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port300Nov14_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port300Nov14_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "smallestunit-fractionalseconddigits.js")]
    public Task Port300Nov14_smallestunit_fractionalseconddigits() => ExecutionTestFromFile("smallestunit-fractionalseconddigits");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port300Nov14_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port300Nov14_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-undefined.js")]
    public Task Port300Nov14_smallestunit_undefined() => ExecutionTestFromFile("smallestunit-undefined");

    [Fact(DisplayName = "smallestunit-valid-units.js")]
    public Task Port300Nov14_smallestunit_valid_units() => ExecutionTestFromFile("smallestunit-valid-units");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port300Nov14_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "throws-when-rounded-duration-is-invalid.js")]
    public Task Port300Nov14_throws_when_rounded_duration_is_invalid() => ExecutionTestFromFile("throws-when-rounded-duration-is-invalid");

    [Fact(DisplayName = "total-of-duration-time-units-out-of-range.js")]
    public Task Port300Nov14_total_of_duration_time_units_out_of_range() => ExecutionTestFromFile("total-of-duration-time-units-out-of-range");

}
