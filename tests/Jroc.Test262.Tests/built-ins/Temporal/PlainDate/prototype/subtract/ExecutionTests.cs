using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.subtract;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.subtract") { }

    [Fact(DisplayName = "argument-duration-max-plus-min-date.js")]
    public Task Port300Nov19_argument_duration_max_plus_min_date() => ExecutionTestFromFile("argument-duration-max-plus-min-date");

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port300Nov19_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port300Nov19_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-invalid-property.js")]
    public Task Port300Nov19_argument_invalid_property() => ExecutionTestFromFile("argument-invalid-property");

    [Fact(DisplayName = "argument-mixed-sign.js")]
    public Task Port300Nov19_argument_mixed_sign() => ExecutionTestFromFile("argument-mixed-sign");

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port300Nov19_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300Nov19_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-singular-properties.js")]
    public Task Port300Nov19_argument_singular_properties() => ExecutionTestFromFile("argument-singular-properties");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port300Nov19_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port300Nov19_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "balance-smaller-units-basic.js")]
    public Task Port300Nov19_balance_smaller_units_basic() => ExecutionTestFromFile("balance-smaller-units-basic");

    [Fact(DisplayName = "balance-smaller-units.js")]
    public Task Port300Nov19_balance_smaller_units() => ExecutionTestFromFile("balance-smaller-units");

    [Fact(DisplayName = "basic-arithmetic.js")]
    public Task Port300Nov19_basic_arithmetic() => ExecutionTestFromFile("basic-arithmetic");

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov19_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov19_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov19_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov19_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov19_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov19_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "limits.js")]
    public Task Port300Nov19_limits() => ExecutionTestFromFile("limits");

    [Fact(DisplayName = "month-boundary.js")]
    public Task Port300Nov19_month_boundary() => ExecutionTestFromFile("month-boundary");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov19_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300Nov19_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port300Nov19_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov19_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-object.js")]
    public Task Port300Nov19_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port300Nov19_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port300Nov19_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port300Nov19_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov19_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "overflow-constrain.js")]
    public Task Port300Nov19_overflow_constrain() => ExecutionTestFromFile("overflow-constrain");

    [Fact(DisplayName = "overflow-invalid-string.js")]
    public Task Port300Nov19_overflow_invalid_string() => ExecutionTestFromFile("overflow-invalid-string");

    [Fact(DisplayName = "overflow-reject.js")]
    public Task Port300Nov19_overflow_reject() => ExecutionTestFromFile("overflow-reject");

    [Fact(DisplayName = "overflow-subtracting-months-from-min-year.js")]
    public Task Port300Nov19_overflow_subtracting_months_from_min_year() => ExecutionTestFromFile("overflow-subtracting-months-from-min-year");

    [Fact(DisplayName = "overflow-undefined.js")]
    public Task Port300Nov19_overflow_undefined() => ExecutionTestFromFile("overflow-undefined");

    [Fact(DisplayName = "overflow-wrong-type.js")]
    public Task Port300Nov19_overflow_wrong_type() => ExecutionTestFromFile("overflow-wrong-type");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov19_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300Nov19_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");
}
