using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.member_expression;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.member_expression") { }

    [Fact(DisplayName = "computed-reference-null-or-undefined", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task computed_reference_null_or_undefined()
        => ExecutionTest("computed-reference-null-or-undefined");
}
