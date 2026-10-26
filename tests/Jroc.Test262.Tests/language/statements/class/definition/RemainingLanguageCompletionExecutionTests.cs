namespace Jroc.Test262.Tests.language.statements.test_class.definition;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.definition") { }

    [Fact(DisplayName = "constructable-but-no-prototype.js")]
    public Task constructable_but_no_prototype()
        => ExecutionTest("constructable-but-no-prototype");

    [Fact(DisplayName = "methods-async-super-call-body.js")]
    public Task methods_async_super_call_body()
        => ExecutionTest("methods-async-super-call-body");

    [Fact(DisplayName = "methods-async-super-call-param.js")]
    public Task methods_async_super_call_param()
        => ExecutionTest("methods-async-super-call-param");

    [Fact(DisplayName = "prototype-getter.js")]
    public Task prototype_getter()
        => ExecutionTest("prototype-getter");

    [Fact(DisplayName = "prototype-setter.js")]
    public Task prototype_setter()
        => ExecutionTest("prototype-setter");

    [Fact(DisplayName = "this-access-restriction.js")]
    public Task this_access_restriction()
        => ExecutionTest("this-access-restriction");
}
