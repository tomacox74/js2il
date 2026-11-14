using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype") { }

    [Fact(DisplayName = "constructor.js")]
    public Task Port1000_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port1000_prop_desc() => ExecutionTestFromFile("prop-desc");

}
