using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.subtract;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.subtract") { }

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port300Nov14_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port300Nov14_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-duration-precision-exact-numerical-values.js")]
    public Task Port300Nov14_argument_duration_precision_exact_numerical_values() => ExecutionTestFromFile("argument-duration-precision-exact-numerical-values");

    [Fact(DisplayName = "argument-invalid-property.js")]
    public Task Port300Nov14_argument_invalid_property() => ExecutionTestFromFile("argument-invalid-property");

    [Fact(DisplayName = "argument-mixed-sign.js")]
    public Task Port300Nov14_argument_mixed_sign() => ExecutionTestFromFile("argument-mixed-sign");

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port300Nov14_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300Nov14_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-singular-properties.js")]
    public Task Port300Nov14_argument_singular_properties() => ExecutionTestFromFile("argument-singular-properties");

    [Fact(DisplayName = "argument-string-fractional-units-rounding-mode.js")]
    public Task Port300Nov14_argument_string_fractional_units_rounding_mode() => ExecutionTestFromFile("argument-string-fractional-units-rounding-mode");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port300Nov14_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port300Nov14_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "balance-negative-result.js")]
    public Task Port300Nov14_balance_negative_result() => ExecutionTestFromFile("balance-negative-result");

    [Fact(DisplayName = "balance-negative-time-units.js")]
    public Task Port300Nov14_balance_negative_time_units() => ExecutionTestFromFile("balance-negative-time-units");

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov14_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov14_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "float64-representable-integer.js")]
    public Task Port300Nov14_float64_representable_integer() => ExecutionTestFromFile("float64-representable-integer");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov14_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nanoseconds-is-number-max-safe-integer.js")]
    public Task Port300Nov14_nanoseconds_is_number_max_safe_integer() => ExecutionTestFromFile("nanoseconds-is-number-max-safe-integer");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300Nov14_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "no-calendar-units.js")]
    public Task Port300Nov14_no_calendar_units() => ExecutionTestFromFile("no-calendar-units");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port300Nov14_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov14_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "precision-exact-mathematical-values.js")]
    public Task Port300Nov14_precision_exact_mathematical_values() => ExecutionTestFromFile("precision-exact-mathematical-values");

    [Fact(DisplayName = "precision-no-floating-point-loss.js")]
    public Task Port300Nov14_precision_no_floating_point_loss() => ExecutionTestFromFile("precision-no-floating-point-loss");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "result-out-of-range-1.js")]
    public Task Port300Nov14_result_out_of_range_1() => ExecutionTestFromFile("result-out-of-range-1");

    [Fact(DisplayName = "result-out-of-range-2.js")]
    public Task Port300Nov14_result_out_of_range_2() => ExecutionTestFromFile("result-out-of-range-2");

    [Fact(DisplayName = "result-out-of-range-3.js")]
    public Task Port300Nov14_result_out_of_range_3() => ExecutionTestFromFile("result-out-of-range-3");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300Nov14_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

}
