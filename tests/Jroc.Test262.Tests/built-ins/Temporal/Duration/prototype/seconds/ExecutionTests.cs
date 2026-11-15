using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.seconds;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.seconds") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov14_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov14_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

}
