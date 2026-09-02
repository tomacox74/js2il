using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.LOG2E;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.LOG2E") { }

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
