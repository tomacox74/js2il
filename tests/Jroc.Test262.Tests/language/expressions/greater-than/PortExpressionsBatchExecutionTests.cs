using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.greater_than;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.greater_than") { }

    [Fact(DisplayName = "S11.8.2_A2.1_T3")]
    public Task S11_8_2_A2_1_T3()
        => ExecutionTest("S11.8.2_A2.1_T3");

    [Fact(DisplayName = "S11.8.2_A2.4_T2")]
    public Task S11_8_2_A2_4_T2()
        => ExecutionTest("S11.8.2_A2.4_T2");

    [Fact(DisplayName = "S11.8.2_A3.1_T2.1")]
    public Task S11_8_2_A3_1_T2_1()
        => ExecutionTest("S11.8.2_A3.1_T2.1");

    [Fact(DisplayName = "S11.8.2_A4.2")]
    public Task S11_8_2_A4_2()
        => ExecutionTest("S11.8.2_A4.2");

    [Fact(DisplayName = "11.8.2-1")]
    public Task _11_8_2_1()
        => ExecutionTest("11.8.2-1");

    [Fact(DisplayName = "11.8.2-2")]
    public Task _11_8_2_2()
        => ExecutionTest("11.8.2-2");

    [Fact(DisplayName = "11.8.2-3")]
    public Task _11_8_2_3()
        => ExecutionTest("11.8.2-3");

    [Fact(DisplayName = "11.8.2-4")]
    public Task _11_8_2_4()
        => ExecutionTest("11.8.2-4");

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");
}
