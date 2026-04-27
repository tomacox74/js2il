using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.coalesce;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.coalesce") { }

    [Fact(DisplayName = "chainable-if-parenthesis-covered-logical-and")]
    public Task chainable_if_parenthesis_covered_logical_and()
        => ExecutionTest("chainable-if-parenthesis-covered-logical-and");

    [Fact(DisplayName = "chainable-if-parenthesis-covered-logical-or")]
    public Task chainable_if_parenthesis_covered_logical_or()
        => ExecutionTest("chainable-if-parenthesis-covered-logical-or");

    [Fact(DisplayName = "chainable-with-bitwise-and")]
    public Task chainable_with_bitwise_and()
        => ExecutionTest("chainable-with-bitwise-and");
}
