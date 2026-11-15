using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.toJSON;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.toJSON") { }

    [Fact(DisplayName = "balance-subseconds.js")]
    public Task Port300Nov14_balance_subseconds() => ExecutionTestFromFile("balance-subseconds");

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov14_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

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

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options.js")]
    public Task Port300Nov14_options() => ExecutionTestFromFile("options");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

}
