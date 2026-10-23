namespace Jroc.Test262.Tests.language.statements.test_class.async_gen_method.forbidden_ext.b2;

public sealed class ClassGeneratorMethodsExecutionTests : DiskExecutionTestsBase
{
    public ClassGeneratorMethodsExecutionTests() : base("language.statements.test_class.async_gen_method.forbidden_ext.b2") { }

    [Fact(DisplayName = "cls-decl-async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task cls_decl_async_gen_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("cls-decl-async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "cls-decl-async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task cls_decl_async_gen_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("cls-decl-async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "cls-decl-async-gen-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task cls_decl_async_gen_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("cls-decl-async-gen-meth-forbidden-ext-indirect-access-prop-caller");
}
