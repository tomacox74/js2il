using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.greater_than;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.greater_than") { }

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
