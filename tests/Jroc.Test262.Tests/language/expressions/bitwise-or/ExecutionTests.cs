using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.bitwise_or;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.bitwise_or") { }

    [Fact(DisplayName = "bigint-non-primitive")]
    public Task bigint_non_primitive()
        => ExecutionTest("bigint-non-primitive");

    [Fact(DisplayName = "bigint-toprimitive")]
    public Task bigint_toprimitive()
        => ExecutionTest("bigint-toprimitive");

    [Fact(DisplayName = "bigint-wrapped-values")]
    public Task bigint_wrapped_values()
        => ExecutionTest("bigint-wrapped-values");
}
