namespace Jroc.Test262.Tests.language.statements.test_class.definition;

public sealed class ClassMethodsExecutionTests : DiskExecutionTestsBase
{
    public ClassMethodsExecutionTests() : base("language.statements.test_class.definition") { }

    [Fact(DisplayName = "fn-name-accessor-get.js")]
    public Task fn_name_accessor_get()
        => ExecutionTest("fn-name-accessor-get");

    [Fact(DisplayName = "fn-name-accessor-set.js")]
    public Task fn_name_accessor_set()
        => ExecutionTest("fn-name-accessor-set");
}
