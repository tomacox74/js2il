using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.boolean;

public class LiteralsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LiteralsConformanceBatchExecutionTests() : base("language.literals.boolean") { }

    [Fact(DisplayName = "S7.8.2_A1_T1.js")]
    public Task S7_8_2_A1_T1()
        => ExecutionTest("S7.8.2_A1_T1");

    [Fact(DisplayName = "S7.8.2_A1_T2.js")]
    public Task S7_8_2_A1_T2()
        => ExecutionTest("S7.8.2_A1_T2");

}
