using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.subtract;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype.subtract") { }

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port1000_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port1000_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-duration-precision-exact-numerical-values.js")]
    public Task Port1000_argument_duration_precision_exact_numerical_values() => ExecutionTestFromFile("argument-duration-precision-exact-numerical-values");

    [Fact(DisplayName = "argument-duration.js")]
    public Task Port1000_argument_duration() => ExecutionTestFromFile("argument-duration");

    [Fact(DisplayName = "argument-higher-units.js")]
    public Task Port1000_argument_higher_units() => ExecutionTestFromFile("argument-higher-units");

    [Fact(DisplayName = "argument-invalid-property.js")]
    public Task Port1000_argument_invalid_property() => ExecutionTestFromFile("argument-invalid-property");

    [Fact(DisplayName = "argument-mixed-sign.js")]
    public Task Port1000_argument_mixed_sign() => ExecutionTestFromFile("argument-mixed-sign");

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port1000_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "argument-object.js")]
    public Task Port1000_argument_object() => ExecutionTestFromFile("argument-object");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port1000_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-singular-properties.js")]
    public Task Port1000_argument_singular_properties() => ExecutionTestFromFile("argument-singular-properties");

    [Fact(DisplayName = "argument-string-duration-too-large.js")]
    public Task Port1000_argument_string_duration_too_large() => ExecutionTestFromFile("argument-string-duration-too-large");

    [Fact(DisplayName = "argument-string-fractional-units-rounding-mode.js")]
    public Task Port1000_argument_string_fractional_units_rounding_mode() => ExecutionTestFromFile("argument-string-fractional-units-rounding-mode");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port1000_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port1000_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "balance-negative-time-units.js")]
    public Task Port1000_balance_negative_time_units() => ExecutionTestFromFile("balance-negative-time-units");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port1000_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port1000_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port1000_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port1000_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port1000_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "options-ignored.js")]
    public Task Port1000_options_ignored() => ExecutionTestFromFile("options-ignored");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port1000_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "precision-exact-mathematical-values-1.js")]
    public Task Port1000_precision_exact_mathematical_values_1() => ExecutionTestFromFile("precision-exact-mathematical-values-1");

    [Fact(DisplayName = "precision-exact-mathematical-values-2.js")]
    public Task Port1000_precision_exact_mathematical_values_2() => ExecutionTestFromFile("precision-exact-mathematical-values-2");

    [Fact(DisplayName = "subtract-large-subseconds.js")]
    public Task Port1000_subtract_large_subseconds() => ExecutionTestFromFile("subtract-large-subseconds");

}
