namespace Jroc.Test262.Tests.language.statements.test_class.decorator.syntax.class_valid;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.decorator.syntax.class_valid") { }

    [Fact(DisplayName = "decorator-member-expr-private-identifier.js")]
    public Task decorator_member_expr_private_identifier()
        => ExecutionTest("decorator-member-expr-private-identifier");
}
