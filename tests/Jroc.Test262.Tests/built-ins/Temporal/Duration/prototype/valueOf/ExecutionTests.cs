using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.valueOf;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.valueOf") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov14_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

}
