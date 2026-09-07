using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.@null;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.null") { }

    [Fact(DisplayName = "S7.8.1_A1_T1.js")]
    public Task S7_8_1_A1_T1()
        => ExecutionTest("S7.8.1_A1_T1");

    [Fact(DisplayName = "S7.8.1_A1_T2.js")]
    public Task S7_8_1_A1_T2()
        => ExecutionTest("S7.8.1_A1_T2");

}
