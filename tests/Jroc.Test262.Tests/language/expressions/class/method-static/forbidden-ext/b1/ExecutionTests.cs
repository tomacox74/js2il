using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.class_.method_static.forbidden_ext.b1;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class.method_static.forbidden_ext.b1") { }

    [Fact(DisplayName = "cls-expr-meth-static-forbidden-ext-direct-access-prop-arguments")]
    public Task cls_expr_meth_static_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("cls-expr-meth-static-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-expr-meth-static-forbidden-ext-direct-access-prop-caller")]
    public Task cls_expr_meth_static_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("cls-expr-meth-static-forbidden-ext-direct-access-prop-caller");
}
