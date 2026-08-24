using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.equals;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.equals") { }

    [Fact(DisplayName = "bigint-and-boolean")]
    public Task bigint_and_boolean()
        => ExecutionTest("bigint-and-boolean");

    [Fact(DisplayName = "bigint-and-incomparable-primitive")]
    public Task bigint_and_incomparable_primitive()
        => ExecutionTest("bigint-and-incomparable-primitive");

    [Fact(DisplayName = "bigint-and-non-finite")]
    public Task bigint_and_non_finite()
        => ExecutionTest("bigint-and-non-finite");

    [Fact(DisplayName = "S11.9.1_A7.8")]
    public Task S11_9_1_A7_8()
        => ExecutionTest("S11.9.1_A7.8");

    [Fact(DisplayName = "S11.9.1_A7.9")]
    public Task S11_9_1_A7_9()
        => ExecutionTest("S11.9.1_A7.9");
}
