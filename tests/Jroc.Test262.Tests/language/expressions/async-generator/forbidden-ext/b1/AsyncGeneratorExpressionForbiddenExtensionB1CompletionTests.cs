namespace Jroc.Test262.Tests.language.expressions.async_generator.forbidden_ext.b1;

public sealed class AsyncGeneratorExpressionForbiddenExtensionB1CompletionTests : DiskExecutionTestsBase
{
    public AsyncGeneratorExpressionForbiddenExtensionB1CompletionTests() : base("language.expressions.async_generator.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-gen-func-expr-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_gen_func_expr_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-gen-func-expr-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-gen-func-expr-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_gen_func_expr_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-gen-func-expr-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "async-gen-named-func-expr-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_gen_named_func_expr_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-gen-named-func-expr-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-gen-named-func-expr-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_gen_named_func_expr_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-gen-named-func-expr-forbidden-ext-direct-access-prop-caller");
}
