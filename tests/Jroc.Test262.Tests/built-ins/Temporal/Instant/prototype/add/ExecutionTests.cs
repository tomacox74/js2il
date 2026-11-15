using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.add;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.add") { }

    [Fact(DisplayName = "add-large-subseconds.js")]
    public Task Port300InstantNov15_add_large_subseconds() => ExecutionTestFromFile("add-large-subseconds");

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port300InstantNov15_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port300InstantNov15_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-invalid-property.js")]
    public Task Port300InstantNov15_argument_invalid_property() => ExecutionTestFromFile("argument-invalid-property");

    [Fact(DisplayName = "argument-mixed-sign.js")]
    public Task Port300InstantNov15_argument_mixed_sign() => ExecutionTestFromFile("argument-mixed-sign");

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port300InstantNov15_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300InstantNov15_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-singular-properties.js")]
    public Task Port300InstantNov15_argument_singular_properties() => ExecutionTestFromFile("argument-singular-properties");

    [Fact(DisplayName = "argument-string-fractional-units-rounding-mode.js")]
    public Task Port300InstantNov15_argument_string_fractional_units_rounding_mode() => ExecutionTestFromFile("argument-string-fractional-units-rounding-mode");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port300InstantNov15_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "argument-string.js")]
    public Task Port300InstantNov15_argument_string() => ExecutionTestFromFile("argument-string");

    [Fact(DisplayName = "basic.js")]
    public Task Port300InstantNov15_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300InstantNov15_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300InstantNov15_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "cross-epoch.js")]
    public Task Port300InstantNov15_cross_epoch() => ExecutionTestFromFile("cross-epoch");

    [Fact(DisplayName = "disallowed-duration-units.js")]
    public Task Port300InstantNov15_disallowed_duration_units() => ExecutionTestFromFile("disallowed-duration-units");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300InstantNov15_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "minimum-maximum-instant.js")]
    public Task Port300InstantNov15_minimum_maximum_instant() => ExecutionTestFromFile("minimum-maximum-instant");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300InstantNov15_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port300InstantNov15_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300InstantNov15_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300InstantNov15_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "result-out-of-range.js")]
    public Task Port300InstantNov15_result_out_of_range() => ExecutionTestFromFile("result-out-of-range");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300InstantNov15_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");
}
