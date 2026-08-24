using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.exponentiation;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.exponentiation") { }

    [Fact(DisplayName = "applying-the-exp-operator_A7")]
    public Task applying_the_exp_operator_A7()
        => ExecutionTest("applying-the-exp-operator_A7");

    [Fact(DisplayName = "applying-the-exp-operator_A8")]
    public Task applying_the_exp_operator_A8()
        => ExecutionTest("applying-the-exp-operator_A8");
}
