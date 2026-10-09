namespace Jroc.Test262.Tests.built_ins.Object.prototype.isPrototypeOf;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.isPrototypeOf") { }

    [Fact(DisplayName = "built-ins/Object/prototype/isPrototypeOf/arg-is-proxy.js")]
    public Task arg_is_proxy() => ExecutionTestFromFile("arg-is-proxy");

    [Fact(DisplayName = "built-ins/Object/prototype/isPrototypeOf/name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "built-ins/Object/prototype/isPrototypeOf/null-this-and-primitive-arg-returns-false.js")]
    public Task null_this_and_primitive_arg_returns_false()
        => ExecutionTestFromFile("null-this-and-primitive-arg-returns-false");

    [Fact(DisplayName = "built-ins/Object/prototype/isPrototypeOf/undefined-this-and-primitive-arg-returns-false.js")]
    public Task undefined_this_and_primitive_arg_returns_false()
        => ExecutionTestFromFile("undefined-this-and-primitive-arg-returns-false");
}
