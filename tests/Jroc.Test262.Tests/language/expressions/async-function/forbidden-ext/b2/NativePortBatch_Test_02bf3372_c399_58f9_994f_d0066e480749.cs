using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_function.forbidden_ext.b2;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.language.expressions.async_function.forbidden_ext.b2") { }

    [Fact(DisplayName = "async-func-expr-named-forbidden-ext-indirect-access-own-prop-caller-get")]
    public Task async_func_expr_named_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTestFromFile("async-func-expr-named-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-func-expr-named-forbidden-ext-indirect-access-own-prop-caller-value")]
    public Task async_func_expr_named_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTestFromFile("async-func-expr-named-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-func-expr-named-forbidden-ext-indirect-access-prop-caller")]
    public Task async_func_expr_named_forbidden_ext_indirect_access_prop_caller() => ExecutionTestFromFile("async-func-expr-named-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "async-func-expr-nameless-forbidden-ext-indirect-access-own-prop-caller-get")]
    public Task async_func_expr_nameless_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTestFromFile("async-func-expr-nameless-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-func-expr-nameless-forbidden-ext-indirect-access-own-prop-caller-value")]
    public Task async_func_expr_nameless_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTestFromFile("async-func-expr-nameless-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-func-expr-nameless-forbidden-ext-indirect-access-prop-caller")]
    public Task async_func_expr_nameless_forbidden_ext_indirect_access_prop_caller() => ExecutionTestFromFile("async-func-expr-nameless-forbidden-ext-indirect-access-prop-caller");

}
