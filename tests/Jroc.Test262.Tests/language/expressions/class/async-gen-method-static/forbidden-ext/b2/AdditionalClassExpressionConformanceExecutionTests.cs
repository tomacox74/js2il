using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.async_gen_method_static.forbidden_ext.b2;

public class AdditionalClassExpressionConformanceExecutionTests : ExecutionTestsBase
{
    public AdditionalClassExpressionConformanceExecutionTests() : base("language.expressions.class_.async-gen-method-static.forbidden-ext.b2") { }

    [Fact(DisplayName = "cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task ported_cls_expr_async_gen_meth_static_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTest("cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task ported_cls_expr_async_gen_meth_static_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTest("cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-prop-caller.js")]
    public Task ported_cls_expr_async_gen_meth_static_forbidden_ext_indirect_access_prop_caller() => ExecutionTest("cls-expr-async-gen-meth-static-forbidden-ext-indirect-access-prop-caller");

}
