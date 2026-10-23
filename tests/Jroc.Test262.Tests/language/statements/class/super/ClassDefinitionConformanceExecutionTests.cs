namespace Jroc.Test262.Tests.language.statements.test_class.super;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.super") { }

    [Fact(DisplayName = "in-getter.js")]
    public Task in_getter()
        => ExecutionTest("in-getter");

    [Fact(DisplayName = "in-methods.js")]
    public Task in_methods()
        => ExecutionTest("in-methods");

    [Fact(DisplayName = "in-setter.js")]
    public Task in_setter()
        => ExecutionTest("in-setter");

    [Fact(DisplayName = "in-static-getter.js")]
    public Task in_static_getter()
        => ExecutionTest("in-static-getter");

    [Fact(DisplayName = "in-static-methods.js")]
    public Task in_static_methods()
        => ExecutionTest("in-static-methods");

    [Fact(DisplayName = "in-static-setter.js")]
    public Task in_static_setter()
        => ExecutionTest("in-static-setter");
}
