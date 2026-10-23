namespace Jroc.Test262.Tests.language.statements.test_class.elements.private_methods;

public sealed class ClassElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassElementsExecutionTests() : base("language.statements.test_class.elements.private_methods") { }

    [Fact(DisplayName = "prod-private-generator.js")]
    public Task prod_private_generator()
        => ExecutionTest("prod-private-generator");

    [Fact(DisplayName = "prod-private-method-initialize-order.js")]
    public Task prod_private_method_initialize_order()
        => ExecutionTest("prod-private-method-initialize-order");

    [Fact(DisplayName = "prod-private-method.js")]
    public Task prod_private_method()
        => ExecutionTest("prod-private-method");
}
