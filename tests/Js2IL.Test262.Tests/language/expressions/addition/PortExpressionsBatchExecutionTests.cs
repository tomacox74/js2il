using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.addition;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.addition") { }

    [Fact(DisplayName = "bigint-and-number")]
    public Task bigint_and_number()
        => ExecutionTest("bigint-and-number");

    [Fact(DisplayName = "bigint-arithmetic")]
    public Task bigint_arithmetic()
        => ExecutionTest("bigint-arithmetic");

    [Fact(DisplayName = "symbol-to-string")]
    public Task symbol_to_string()
        => ExecutionTest("symbol-to-string");
}
