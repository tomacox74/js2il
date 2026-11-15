using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.toJSON;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.toJSON") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300InstantNov15_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300InstantNov15_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "fromEpochMilliseconds.js")]
    public Task Port300InstantNov15_fromEpochMilliseconds() => ExecutionTestFromFile("fromEpochMilliseconds");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-epochnanoseconds.js")]
    public Task Port300InstantNov15_negative_epochnanoseconds() => ExecutionTestFromFile("negative-epochnanoseconds");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300InstantNov15_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "year-format.js")]
    public Task Port300InstantNov15_year_format() => ExecutionTestFromFile("year-format");
}
