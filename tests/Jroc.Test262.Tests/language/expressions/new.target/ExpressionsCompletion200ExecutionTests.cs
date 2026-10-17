namespace Jroc.Test262.Tests.language.expressions.new_target;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.new_target") { }

    [Fact(DisplayName = "unary-expr.js")]
    public Task unary_expr()
        => ExecutionTest("unary-expr");

    [Fact(DisplayName = "value-via-super-call.js")]
    public Task value_via_super_call()
        => ExecutionTest("value-via-super-call");

    [Fact(DisplayName = "value-via-super-property.js")]
    public Task value_via_super_property()
        => ExecutionTest("value-via-super-property");
}
