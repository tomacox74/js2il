using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.nanoseconds;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.nanoseconds") { }

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov14_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "nanoseconds.js")]
    public Task Port300Nov14_nanoseconds() => ExecutionTestFromFile("nanoseconds");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

}
