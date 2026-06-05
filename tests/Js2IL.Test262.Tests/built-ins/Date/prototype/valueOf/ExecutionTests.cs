using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Date.prototype.valueOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.valueOf") { }

    [Fact(DisplayName = "S9.4_A3_T1")]
    public Task S9_4_A3_T1()
        => ExecutionTestFromFile("S9.4_A3_T1");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
