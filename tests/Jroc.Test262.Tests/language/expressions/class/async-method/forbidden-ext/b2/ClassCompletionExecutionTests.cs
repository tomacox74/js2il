using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.async_method.forbidden_ext.b2;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.async_method.forbidden_ext.b2") { }

    [Fact(DisplayName = "cls-expr-async-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task cls_expr_async_meth_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTestFromFile("cls-expr-async-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "cls-expr-async-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task cls_expr_async_meth_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTestFromFile("cls-expr-async-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "cls-expr-async-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task cls_expr_async_meth_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTestFromFile("cls-expr-async-meth-forbidden-ext-indirect-access-prop-caller");
}
