using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.toLocaleString;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.toLocaleString") { }

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov19_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov19_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov19_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov19_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov19_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov19_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-string.js")]
    public Task Port300Nov19_return_string() => ExecutionTestFromFile("return-string");
}
