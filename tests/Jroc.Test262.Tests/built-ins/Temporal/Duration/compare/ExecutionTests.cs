using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.compare;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.compare") { }

    [Fact(DisplayName = "argument-cast.js")]
    public Task Port300Nov14_argument_cast() => ExecutionTestFromFile("argument-cast");

    [Fact(DisplayName = "argument-duration-max.js")]
    public Task Port300Nov14_argument_duration_max() => ExecutionTestFromFile("argument-duration-max");

    [Fact(DisplayName = "argument-duration-out-of-range.js")]
    public Task Port300Nov14_argument_duration_out_of_range() => ExecutionTestFromFile("argument-duration-out-of-range");

    [Fact(DisplayName = "argument-duration-precision-exact-numerical-values.js")]
    public Task Port300Nov14_argument_duration_precision_exact_numerical_values() => ExecutionTestFromFile("argument-duration-precision-exact-numerical-values");

    [Fact(DisplayName = "argument-propertybag-optional-properties.js")]
    public Task Port300Nov14_argument_propertybag_optional_properties() => ExecutionTestFromFile("argument-propertybag-optional-properties");

    [Fact(DisplayName = "argument-string-fractional-units-rounding-mode.js")]
    public Task Port300Nov14_argument_string_fractional_units_rounding_mode() => ExecutionTestFromFile("argument-string-fractional-units-rounding-mode");

    [Fact(DisplayName = "argument-string-negative-fractional-units.js")]
    public Task Port300Nov14_argument_string_negative_fractional_units() => ExecutionTestFromFile("argument-string-negative-fractional-units");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "compare-no-precision-loss.js")]
    public Task Port300Nov14_compare_no_precision_loss() => ExecutionTestFromFile("compare-no-precision-loss");

    [Fact(DisplayName = "instances-identical.js")]
    public Task Port300Nov14_instances_identical() => ExecutionTestFromFile("instances-identical");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

}
