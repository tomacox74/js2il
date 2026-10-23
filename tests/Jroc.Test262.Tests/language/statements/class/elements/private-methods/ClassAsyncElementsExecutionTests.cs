namespace Jroc.Test262.Tests.language.statements.test_class.elements.private_methods;

public sealed class ClassAsyncElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassAsyncElementsExecutionTests() : base("language.statements.test_class.elements.private_methods") { }

    [Fact(DisplayName = "prod-private-async-generator.js")]
    public Task prod_private_async_generator()
        => ExecutionTest("prod-private-async-generator");

    [Fact(DisplayName = "prod-private-async-method.js")]
    public Task prod_private_async_method()
        => ExecutionTest("prod-private-async-method");
}
