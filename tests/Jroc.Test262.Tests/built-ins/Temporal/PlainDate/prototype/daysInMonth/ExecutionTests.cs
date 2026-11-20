using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.daysInMonth;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.daysInMonth") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov19_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov19_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov19_prop_desc() => ExecutionTestFromFile("prop-desc");
}
