using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant") { }

    [Fact(DisplayName = "argument.js")]
    public Task Port300InstantNov15_argument() => ExecutionTestFromFile("argument");

    [Fact(DisplayName = "basic.js")]
    public Task Port300InstantNov15_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "constructor.js")]
    public Task Port300InstantNov15_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "get-prototype-from-constructor-throws.js")]
    public Task Port300InstantNov15_get_prototype_from_constructor_throws() => ExecutionTestFromFile("get-prototype-from-constructor-throws");

    [Fact(DisplayName = "large-bigint.js")]
    public Task Port300InstantNov15_large_bigint() => ExecutionTestFromFile("large-bigint");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "limits.js")]
    public Task Port300InstantNov15_limits() => ExecutionTestFromFile("limits");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");
}
