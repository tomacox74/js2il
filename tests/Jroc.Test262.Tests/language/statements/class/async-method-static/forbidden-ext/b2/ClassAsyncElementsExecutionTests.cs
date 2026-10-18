namespace Jroc.Test262.Tests.language.statements.test_class.async_method_static.forbidden_ext.b2;

public sealed class ClassAsyncElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassAsyncElementsExecutionTests() : base("language.statements.test_class.async_method_static.forbidden_ext.b2") { }

    [Fact(DisplayName = "cls-decl-async-meth-static-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task cls_decl_async_meth_static_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("cls-decl-async-meth-static-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "cls-decl-async-meth-static-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task cls_decl_async_meth_static_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("cls-decl-async-meth-static-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "cls-decl-async-meth-static-forbidden-ext-indirect-access-prop-caller.js")]
    public Task cls_decl_async_meth_static_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("cls-decl-async-meth-static-forbidden-ext-indirect-access-prop-caller");
}
