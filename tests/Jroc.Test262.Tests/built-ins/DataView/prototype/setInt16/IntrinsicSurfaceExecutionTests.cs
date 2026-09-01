using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setInt16;

public class IntrinsicSurfaceExecutionTests : DiskExecutionTestsBase
{
    public IntrinsicSurfaceExecutionTests() : base("built_ins.DataView.prototype.setInt16") { }

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
}
