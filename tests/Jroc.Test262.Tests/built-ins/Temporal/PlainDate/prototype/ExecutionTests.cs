using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate.prototype") { }

    [Fact(DisplayName = "constructor.js")]
    public Task Port300Nov19_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov19_prop_desc() => ExecutionTestFromFile("prop-desc");
}
