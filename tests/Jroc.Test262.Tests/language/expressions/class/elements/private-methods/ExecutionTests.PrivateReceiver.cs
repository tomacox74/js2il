using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.elements.private_methods;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.elements.private-methods") { }

    [Fact(DisplayName = "prod-private-async-generator.js")]
    public Task private_receiver_prod_private_async_generator() => ExecutionTest("prod-private-async-generator");

    [Fact(DisplayName = "prod-private-async-method.js")]
    public Task private_receiver_prod_private_async_method() => ExecutionTest("prod-private-async-method");

    [Fact(DisplayName = "prod-private-generator.js")]
    public Task private_receiver_prod_private_generator() => ExecutionTest("prod-private-generator");

    [Fact(DisplayName = "prod-private-method-initialize-order.js")]
    public Task private_receiver_prod_private_method_initialize_order() => ExecutionTest("prod-private-method-initialize-order");

    [Fact(DisplayName = "prod-private-method.js")]
    public Task private_receiver_prod_private_method() => ExecutionTest("prod-private-method");

}
