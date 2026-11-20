using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.toStringTag;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.toStringTag") { }

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov17_prop_desc() => ExecutionTestFromFile("prop-desc");
}
