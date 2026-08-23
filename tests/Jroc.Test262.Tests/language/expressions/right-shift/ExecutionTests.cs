using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.right_shift;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.right_shift") { }

    [Fact(DisplayName = "S11.7.2_A5.1_T1")]
    public Task S11_7_2_A5_1_T1()
        => ExecutionTest("S11.7.2_A5.1_T1");
}
