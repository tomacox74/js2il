namespace Jroc.Test262.Tests.language.statements.test_class.gen_method.forbidden_ext.b1;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.gen_method.forbidden_ext.b1") { }

    [Fact(DisplayName = "cls-decl-gen-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task cls_decl_gen_meth_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("cls-decl-gen-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-decl-gen-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task cls_decl_gen_meth_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("cls-decl-gen-meth-forbidden-ext-direct-access-prop-caller");
}
