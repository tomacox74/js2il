using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.call;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "11.2.3-3_1")]
    public Task _11_2_3_3_1()
        => ExecutionTest("11.2.3-3_1");

    [Fact(DisplayName = "11.2.3-3_2")]
    public Task _11_2_3_3_2()
        => ExecutionTest("11.2.3-3_2");

    [Fact(DisplayName = "11.2.3-3_3")]
    public Task _11_2_3_3_3()
        => ExecutionTest("11.2.3-3_3");

    [Fact(DisplayName = "11.2.3-3_4")]
    public Task _11_2_3_3_4()
        => ExecutionTest("11.2.3-3_4");

    [Fact(DisplayName = "11.2.3-3_5", Skip = "Tracked by eval support: JROC still rejects eval at validation time.")]
    public Task _11_2_3_3_5()
        => ExecutionTest("11.2.3-3_5");

    [Fact(DisplayName = "11.2.3-3_6")]
    public Task _11_2_3_3_6()
        => ExecutionTest("11.2.3-3_6");

    [Fact(DisplayName = "11.2.3-3_7")]
    public Task _11_2_3_3_7()
        => ExecutionTest("11.2.3-3_7");

    [Fact(DisplayName = "11.2.3-3_8")]
    public Task _11_2_3_3_8()
        => ExecutionTest("11.2.3-3_8");
}
