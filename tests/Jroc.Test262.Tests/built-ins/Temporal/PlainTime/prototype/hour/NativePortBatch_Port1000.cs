using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.hour;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype.hour") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port1000_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port1000_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port1000_prop_desc() => ExecutionTestFromFile("prop-desc");

}
