using Xunit;

namespace Jroc.Test262.Tests.language.arguments_object;

public sealed class RemainingArgumentsExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public RemainingArgumentsExecutionTests() : base("language.arguments-object")
    {
    }

    [Fact(DisplayName = "language/arguments-object/10.5-1-s.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_10_5_1_s() => ExecutionTest("10.5-1-s");

    [Fact(DisplayName = "language/arguments-object/10.5-7-b-1-s.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_10_5_7_b_1_s() => ExecutionTest("10.5-7-b-1-s");

    [Fact(DisplayName = "language/arguments-object/async-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_async_gen_meth_args_trailing_comma_multiple() => ExecutionTest("async-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/async-gen-meth-args-trailing-comma-null.js")]
    public Task test_async_gen_meth_args_trailing_comma_null() => ExecutionTest("async-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/async-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_async_gen_meth_args_trailing_comma_single_args() => ExecutionTest("async-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/async-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_async_gen_meth_args_trailing_comma_spread_operator() => ExecutionTest("async-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/async-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_async_gen_meth_args_trailing_comma_undefined() => ExecutionTest("async-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/async-gen-named-func-expr-args-trailing-comma-multiple.js")]
    public Task test_async_gen_named_func_expr_args_trailing_comma_multiple() => ExecutionTest("async-gen-named-func-expr-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/async-gen-named-func-expr-args-trailing-comma-null.js")]
    public Task test_async_gen_named_func_expr_args_trailing_comma_null() => ExecutionTest("async-gen-named-func-expr-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/async-gen-named-func-expr-args-trailing-comma-single-args.js")]
    public Task test_async_gen_named_func_expr_args_trailing_comma_single_args() => ExecutionTest("async-gen-named-func-expr-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/async-gen-named-func-expr-args-trailing-comma-spread-operator.js")]
    public Task test_async_gen_named_func_expr_args_trailing_comma_spread_operator() => ExecutionTest("async-gen-named-func-expr-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/async-gen-named-func-expr-args-trailing-comma-undefined.js")]
    public Task test_async_gen_named_func_expr_args_trailing_comma_undefined() => ExecutionTest("async-gen-named-func-expr-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-func-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_async_gen_func_args_trailing_comma_multiple() => ExecutionTest("cls-decl-async-gen-func-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-func-args-trailing-comma-null.js")]
    public Task test_cls_decl_async_gen_func_args_trailing_comma_null() => ExecutionTest("cls-decl-async-gen-func-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-func-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_async_gen_func_args_trailing_comma_single_args() => ExecutionTest("cls-decl-async-gen-func-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-func-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_async_gen_func_args_trailing_comma_spread_operator() => ExecutionTest("cls-decl-async-gen-func-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-func-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_async_gen_func_args_trailing_comma_undefined() => ExecutionTest("cls-decl-async-gen-func-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_async_gen_meth_args_trailing_comma_multiple() => ExecutionTest("cls-decl-async-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_async_gen_meth_args_trailing_comma_null() => ExecutionTest("cls-decl-async-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_async_gen_meth_args_trailing_comma_single_args() => ExecutionTest("cls-decl-async-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_async_gen_meth_args_trailing_comma_spread_operator() => ExecutionTest("cls-decl-async-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_async_gen_meth_args_trailing_comma_undefined() => ExecutionTest("cls-decl-async-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_async_gen_meth_static_args_trailing_comma_multiple() => ExecutionTest("cls-decl-async-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_async_gen_meth_static_args_trailing_comma_null() => ExecutionTest("cls-decl-async-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_async_gen_meth_static_args_trailing_comma_single_args() => ExecutionTest("cls-decl-async-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_async_gen_meth_static_args_trailing_comma_spread_operator() => ExecutionTest("cls-decl-async-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_async_gen_meth_static_args_trailing_comma_undefined() => ExecutionTest("cls-decl-async-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_async_private_gen_meth_args_trailing_comma_multiple() => ExecutionTest("cls-decl-async-private-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_async_private_gen_meth_args_trailing_comma_null() => ExecutionTest("cls-decl-async-private-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_async_private_gen_meth_args_trailing_comma_single_args() => ExecutionTest("cls-decl-async-private-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_async_private_gen_meth_args_trailing_comma_spread_operator() => ExecutionTest("cls-decl-async-private-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_async_private_gen_meth_args_trailing_comma_undefined() => ExecutionTest("cls-decl-async-private-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_async_private_gen_meth_static_args_trailing_comma_multiple() => ExecutionTest("cls-decl-async-private-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_async_private_gen_meth_static_args_trailing_comma_null() => ExecutionTest("cls-decl-async-private-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_async_private_gen_meth_static_args_trailing_comma_single_args() => ExecutionTest("cls-decl-async-private-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_async_private_gen_meth_static_args_trailing_comma_spread_operator() => ExecutionTest("cls-decl-async-private-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-async-private-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_async_private_gen_meth_static_args_trailing_comma_undefined() => ExecutionTest("cls-decl-async-private-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-func-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_async_gen_func_args_trailing_comma_multiple() => ExecutionTest("cls-expr-async-gen-func-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-func-args-trailing-comma-null.js")]
    public Task test_cls_expr_async_gen_func_args_trailing_comma_null() => ExecutionTest("cls-expr-async-gen-func-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-func-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_async_gen_func_args_trailing_comma_single_args() => ExecutionTest("cls-expr-async-gen-func-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-func-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_async_gen_func_args_trailing_comma_spread_operator() => ExecutionTest("cls-expr-async-gen-func-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-func-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_async_gen_func_args_trailing_comma_undefined() => ExecutionTest("cls-expr-async-gen-func-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_async_gen_meth_args_trailing_comma_multiple() => ExecutionTest("cls-expr-async-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_async_gen_meth_args_trailing_comma_null() => ExecutionTest("cls-expr-async-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_async_gen_meth_args_trailing_comma_single_args() => ExecutionTest("cls-expr-async-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_async_gen_meth_args_trailing_comma_spread_operator() => ExecutionTest("cls-expr-async-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_async_gen_meth_args_trailing_comma_undefined() => ExecutionTest("cls-expr-async-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_async_gen_meth_static_args_trailing_comma_multiple() => ExecutionTest("cls-expr-async-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_async_gen_meth_static_args_trailing_comma_null() => ExecutionTest("cls-expr-async-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_async_gen_meth_static_args_trailing_comma_single_args() => ExecutionTest("cls-expr-async-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_async_gen_meth_static_args_trailing_comma_spread_operator() => ExecutionTest("cls-expr-async-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_async_gen_meth_static_args_trailing_comma_undefined() => ExecutionTest("cls-expr-async-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_async_private_gen_meth_args_trailing_comma_multiple() => ExecutionTest("cls-expr-async-private-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_async_private_gen_meth_args_trailing_comma_null() => ExecutionTest("cls-expr-async-private-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_async_private_gen_meth_args_trailing_comma_single_args() => ExecutionTest("cls-expr-async-private-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_async_private_gen_meth_args_trailing_comma_spread_operator() => ExecutionTest("cls-expr-async-private-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_async_private_gen_meth_args_trailing_comma_undefined() => ExecutionTest("cls-expr-async-private-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_async_private_gen_meth_static_args_trailing_comma_multiple() => ExecutionTest("cls-expr-async-private-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_async_private_gen_meth_static_args_trailing_comma_null() => ExecutionTest("cls-expr-async-private-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_async_private_gen_meth_static_args_trailing_comma_single_args() => ExecutionTest("cls-expr-async-private-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_async_private_gen_meth_static_args_trailing_comma_spread_operator() => ExecutionTest("cls-expr-async-private-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-async-private-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_async_private_gen_meth_static_args_trailing_comma_undefined() => ExecutionTest("cls-expr-async-private-gen-meth-static-args-trailing-comma-undefined");
}
