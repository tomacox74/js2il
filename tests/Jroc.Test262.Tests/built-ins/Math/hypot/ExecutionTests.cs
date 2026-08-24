using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.hypot;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.hypot") { }

    [Fact(DisplayName = "Math.hypot_ToNumberErr")]
    public Task Math_hypot_ToNumberErr()
        => ExecutionTestFromFile("Math.hypot_ToNumberErr");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
