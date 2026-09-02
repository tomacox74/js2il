using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math") { }

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

}
