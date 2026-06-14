using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.member_expression;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.member_expression") { }

    [Fact(DisplayName = "computed-reference-null-or-undefined")]
    public Task computed_reference_null_or_undefined()
        => ExecutionTest("computed-reference-null-or-undefined");
}
