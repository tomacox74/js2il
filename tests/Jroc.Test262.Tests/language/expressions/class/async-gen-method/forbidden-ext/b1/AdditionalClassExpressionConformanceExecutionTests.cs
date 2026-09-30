using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.async_gen_method.forbidden_ext.b1;

public class AdditionalClassExpressionConformanceExecutionTests : ExecutionTestsBase
{
    public AdditionalClassExpressionConformanceExecutionTests() : base("language.expressions.class_.async-gen-method.forbidden-ext.b1") { }

    [Fact(DisplayName = "cls-expr-async-gen-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task ported_cls_expr_async_gen_meth_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("cls-expr-async-gen-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-expr-async-gen-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task ported_cls_expr_async_gen_meth_forbidden_ext_direct_access_prop_caller() => ExecutionTest("cls-expr-async-gen-meth-forbidden-ext-direct-access-prop-caller");

}
