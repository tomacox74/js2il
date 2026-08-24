using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.class_.method_static.forbidden_ext.b1;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class.method_static.forbidden_ext.b1") { }

    [Fact(DisplayName = "cls-decl-meth-static-forbidden-ext-direct-access-prop-arguments")]
    public Task cls_decl_meth_static_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("cls-decl-meth-static-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-decl-meth-static-forbidden-ext-direct-access-prop-caller")]
    public Task cls_decl_meth_static_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("cls-decl-meth-static-forbidden-ext-direct-access-prop-caller");
}
