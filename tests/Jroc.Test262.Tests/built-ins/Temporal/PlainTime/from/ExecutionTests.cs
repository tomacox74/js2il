using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.from;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainTime.from") { }

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov17_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov17_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov17_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov17_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov17_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300Nov17_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");
}
