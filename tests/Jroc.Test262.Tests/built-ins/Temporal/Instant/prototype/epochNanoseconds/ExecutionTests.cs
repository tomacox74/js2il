using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.epochNanoseconds;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.epochNanoseconds") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300InstantNov15_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300InstantNov15_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");
}
