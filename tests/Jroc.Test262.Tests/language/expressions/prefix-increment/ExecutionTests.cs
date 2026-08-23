using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.prefix_increment;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.prefix_increment") { }

    [Fact(DisplayName = "S11.4.4_A5_T1")]
    public Task S11_4_4_A5_T1()
        => ExecutionTest("S11.4.4_A5_T1");

    [Fact(DisplayName = "S11.4.4_A5_T2")]
    public Task S11_4_4_A5_T2()
        => ExecutionTest("S11.4.4_A5_T2");

    [Fact(DisplayName = "S11.4.4_A6_T3")]
    public Task S11_4_4_A6_T3()
        => ExecutionTest("S11.4.4_A6_T3");
}
