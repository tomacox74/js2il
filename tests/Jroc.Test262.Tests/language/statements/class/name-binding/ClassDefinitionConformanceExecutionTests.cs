namespace Jroc.Test262.Tests.language.statements.test_class.name_binding;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.name_binding") { }

    [Fact(DisplayName = "expression.js")]
    public Task expression()
        => ExecutionTest("expression");
}
