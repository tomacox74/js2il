namespace Jroc.Test262.Tests.language.statements.test_class.async_gen_method_static.forbidden_ext.b1;

public sealed class ClassGeneratorMethodsExecutionTests : DiskExecutionTestsBase
{
    public ClassGeneratorMethodsExecutionTests() : base("language.statements.test_class.async_gen_method_static.forbidden_ext.b1") { }

    [Fact(DisplayName = "cls-decl-async-gen-meth-static-forbidden-ext-direct-access-prop-arguments.js")]
    public Task cls_decl_async_gen_meth_static_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("cls-decl-async-gen-meth-static-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-decl-async-gen-meth-static-forbidden-ext-direct-access-prop-caller.js")]
    public Task cls_decl_async_gen_meth_static_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("cls-decl-async-gen-meth-static-forbidden-ext-direct-access-prop-caller");
}
