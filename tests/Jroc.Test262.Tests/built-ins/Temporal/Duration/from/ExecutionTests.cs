using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.from;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.from") { }

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port300Nov14_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port300Nov14_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-duration-precision-exact-numerical-values.js")]
    public Task Port300Nov14_argument_duration_precision_exact_numerical_values() => ExecutionTestFromFile("argument-duration-precision-exact-numerical-values");

    [Fact(DisplayName = "argument-duration.js")]
    public Task Port300Nov14_argument_duration() => ExecutionTestFromFile("argument-duration");

    [Fact(DisplayName = "argument-existing-object.js")]
    public Task Port300Nov14_argument_existing_object() => ExecutionTestFromFile("argument-existing-object");

    [Fact(DisplayName = "argument-non-string.js")]
    public Task Port300Nov14_argument_non_string() => ExecutionTestFromFile("argument-non-string");

    [Fact(DisplayName = "argument-object-invalid.js")]
    public Task Port300Nov14_argument_object_invalid() => ExecutionTestFromFile("argument-object-invalid");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300Nov14_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-propertybag.js")]
    public Task Port300Nov14_argument_propertybag() => ExecutionTestFromFile("argument-propertybag");

    [Fact(DisplayName = "argument-string-fractional-precision.js")]
    public Task Port300Nov14_argument_string_fractional_precision() => ExecutionTestFromFile("argument-string-fractional-precision");

    [Fact(DisplayName = "argument-string-fractional-units-rounding-mode.js")]
    public Task Port300Nov14_argument_string_fractional_units_rounding_mode() => ExecutionTestFromFile("argument-string-fractional-units-rounding-mode");

    [Fact(DisplayName = "argument-string-fractional-with-zero-subparts.js")]
    public Task Port300Nov14_argument_string_fractional_with_zero_subparts() => ExecutionTestFromFile("argument-string-fractional-with-zero-subparts");

    [Fact(DisplayName = "argument-string-invalid.js")]
    public Task Port300Nov14_argument_string_invalid() => ExecutionTestFromFile("argument-string-invalid");

    [Fact(DisplayName = "argument-string-is-infinity.js")]
    public Task Port300Nov14_argument_string_is_infinity() => ExecutionTestFromFile("argument-string-is-infinity");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port300Nov14_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port300Nov14_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov14_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "get-property-throws.js")]
    public Task Port300Nov14_get_property_throws() => ExecutionTestFromFile("get-property-throws");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov14_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "invalid-type.js")]
    public Task Port300Nov14_invalid_type() => ExecutionTestFromFile("invalid-type");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "lower-limit.js")]
    public Task Port300Nov14_lower_limit() => ExecutionTestFromFile("lower-limit");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-inifinity-throws-rangeerror.js")]
    public Task Port300Nov14_negative_inifinity_throws_rangeerror() => ExecutionTestFromFile("negative-inifinity-throws-rangeerror");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port300Nov14_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov14_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "string-with-skipped-units.js")]
    public Task Port300Nov14_string_with_skipped_units() => ExecutionTestFromFile("string-with-skipped-units");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300Nov14_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

}
