using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_function.forbidden_ext.b1;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.language.expressions.async_function.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-func-expr-named-forbidden-ext-direct-access-prop-arguments")]
    public Task async_func_expr_named_forbidden_ext_direct_access_prop_arguments() => ExecutionTestFromFile("async-func-expr-named-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-func-expr-named-forbidden-ext-direct-access-prop-caller")]
    public Task async_func_expr_named_forbidden_ext_direct_access_prop_caller() => ExecutionTestFromFile("async-func-expr-named-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "async-func-expr-nameless-forbidden-ext-direct-access-prop-arguments")]
    public Task async_func_expr_nameless_forbidden_ext_direct_access_prop_arguments() => ExecutionTestFromFile("async-func-expr-nameless-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-func-expr-nameless-forbidden-ext-direct-access-prop-caller")]
    public Task async_func_expr_nameless_forbidden_ext_direct_access_prop_caller() => ExecutionTestFromFile("async-func-expr-nameless-forbidden-ext-direct-access-prop-caller");

}
