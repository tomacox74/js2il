using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.conditional;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.conditional") { }

    [Fact(DisplayName = "coalesce-expr-ternary")]
    public Task coalesce_expr_ternary()
        => ExecutionTest("coalesce-expr-ternary");

    [Fact(DisplayName = "in-branch-1")]
    public Task in_branch_1()
        => ExecutionTest("in-branch-1");

    [Fact(DisplayName = "symbol-conditional-evaluation")]
    public Task symbol_conditional_evaluation()
        => ExecutionTest("symbol-conditional-evaluation");
}
