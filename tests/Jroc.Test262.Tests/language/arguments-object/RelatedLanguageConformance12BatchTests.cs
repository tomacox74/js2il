using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.arguments_object;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.arguments_object") { }

    [Fact(DisplayName = "language/arguments-object/10.5-1gs.js")]
    public Task test_10_5_1gs()
        => CompilationFailureTest("10.5-1gs", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/arguments-object/10.6-12-1.js")]
    public Task test_10_6_12_1()
        => ExecutionTest("10.6-12-1");

    [Fact(DisplayName = "language/arguments-object/10.6-13-c-1-s.js")]
    public Task test_10_6_13_c_1_s()
        => ExecutionTest("10.6-13-c-1-s");

    [Fact(DisplayName = "language/arguments-object/10.6-14-c-1-s.js")]
    public Task test_10_6_14_c_1_s()
        => ExecutionTest("10.6-14-c-1-s");

    [Fact(DisplayName = "language/arguments-object/10.6-14-c-4-s.js")]
    public Task test_10_6_14_c_4_s()
        => ExecutionTest("10.6-14-c-4-s");

    [Fact(DisplayName = "language/arguments-object/10.6-2gs.js")]
    public Task test_10_6_2gs()
        => ExecutionTest("10.6-2gs");

    [Fact(DisplayName = "language/arguments-object/10.6-6-2.js")]
    public Task test_10_6_6_2()
        => ExecutionTest("10.6-6-2");

    [Fact(DisplayName = "language/arguments-object/10.6-6-3-s.js")]
    public Task test_10_6_6_3_s()
        => ExecutionTest("10.6-6-3-s");

    [Fact(DisplayName = "language/arguments-object/10.6-6-3.js")]
    public Task test_10_6_6_3()
        => ExecutionTest("10.6-6-3");

    [Fact(DisplayName = "language/arguments-object/10.6-6-4-s.js")]
    public Task test_10_6_6_4_s()
        => ExecutionTest("10.6-6-4-s");

    [Fact(DisplayName = "language/arguments-object/10.6-6-4.js")]
    public Task test_10_6_6_4()
        => ExecutionTest("10.6-6-4");

    [Fact(DisplayName = "language/arguments-object/10.6-7-1.js")]
    public Task test_10_6_7_1()
        => ExecutionTest("10.6-7-1");

    [Fact(DisplayName = "language/arguments-object/S10.1.6_A1_T2.js")]
    public Task test_S10_1_6_A1_T2()
        => ExecutionTest("S10.1.6_A1_T2");

    [Fact(DisplayName = "language/arguments-object/S10.6_A1.js")]
    public Task test_S10_6_A1()
        => ExecutionTest("S10.6_A1");

    [Fact(DisplayName = "language/arguments-object/S10.6_A2.js")]
    public Task test_S10_6_A2()
        => ExecutionTest("S10.6_A2");

    [Fact(DisplayName = "language/arguments-object/S10.6_A3_T1.js")]
    public Task test_S10_6_A3_T1()
        => ExecutionTest("S10.6_A3_T1");

    [Fact(DisplayName = "language/arguments-object/S10.6_A3_T2.js")]
    public Task test_S10_6_A3_T2()
        => ExecutionTest("S10.6_A3_T2");

    [Fact(DisplayName = "language/arguments-object/S10.6_A3_T3.js")]
    public Task test_S10_6_A3_T3()
        => ExecutionTest("S10.6_A3_T3");

    [Fact(DisplayName = "language/arguments-object/S10.6_A3_T4.js")]
    public Task test_S10_6_A3_T4()
        => ExecutionTest("S10.6_A3_T4");

    [Fact(DisplayName = "language/arguments-object/S10.6_A4.js")]
    public Task test_S10_6_A4()
        => ExecutionTest("S10.6_A4");

    [Fact(DisplayName = "language/arguments-object/S10.6_A5_T1.js")]
    public Task test_S10_6_A5_T1()
        => ExecutionTest("S10.6_A5_T1");

    [Fact(DisplayName = "language/arguments-object/S10.6_A5_T2.js")]
    public Task test_S10_6_A5_T2()
        => ExecutionTest("S10.6_A5_T2");

    [Fact(DisplayName = "language/arguments-object/S10.6_A5_T3.js")]
    public Task test_S10_6_A5_T3()
        => ExecutionTest("S10.6_A5_T3");

    [Fact(DisplayName = "language/arguments-object/S10.6_A5_T4.js")]
    public Task test_S10_6_A5_T4()
        => ExecutionTest("S10.6_A5_T4");

    [Fact(DisplayName = "language/arguments-object/S10.6_A6.js")]
    public Task test_S10_6_A6()
        => ExecutionTest("S10.6_A6");

    [Fact(DisplayName = "language/arguments-object/S10.6_A7.js")]
    public Task test_S10_6_A7()
        => ExecutionTest("S10.6_A7");

    [Fact(DisplayName = "language/arguments-object/arguments-caller.js")]
    public Task test_arguments_caller()
        => ExecutionTest("arguments-caller");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_gen_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_gen_meth_args_trailing_comma_null()
        => ExecutionTest("cls-decl-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_gen_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_gen_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_gen_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_gen_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_gen_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-decl-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_gen_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_gen_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_gen_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_meth_args_trailing_comma_null()
        => ExecutionTest("cls-decl-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-decl-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_private_gen_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-private-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_private_gen_meth_args_trailing_comma_null()
        => ExecutionTest("cls-decl-private-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_private_gen_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-private-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_private_gen_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-private-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_private_gen_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-private-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_private_gen_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-private-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_private_gen_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-decl-private-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_private_gen_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-private-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_private_gen_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-private-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_private_gen_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-private-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_private_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-private-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-args-trailing-comma-null.js")]
    public Task test_cls_decl_private_meth_args_trailing_comma_null()
        => ExecutionTest("cls-decl-private-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_private_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-private-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_private_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-private-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_private_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-private-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_decl_private_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-decl-private-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_decl_private_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-decl-private-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_decl_private_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-decl-private-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_decl_private_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-decl-private-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-decl-private-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_decl_private_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-decl-private-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_gen_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_gen_meth_args_trailing_comma_null()
        => ExecutionTest("cls-expr-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_gen_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_gen_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_gen_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_gen_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_gen_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-expr-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_gen_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_gen_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_gen_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_meth_args_trailing_comma_null()
        => ExecutionTest("cls-expr-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-expr-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_private_gen_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-private-gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_private_gen_meth_args_trailing_comma_null()
        => ExecutionTest("cls-expr-private-gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_private_gen_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-private-gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_private_gen_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-private-gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_private_gen_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-private-gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_private_gen_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-private-gen-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_private_gen_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-expr-private-gen-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_private_gen_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-private-gen-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_private_gen_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-private-gen-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-gen-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_private_gen_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-private-gen-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_private_meth_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-private-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-args-trailing-comma-null.js")]
    public Task test_cls_expr_private_meth_args_trailing_comma_null()
        => ExecutionTest("cls-expr-private-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_private_meth_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-private-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_private_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-private-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_private_meth_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-private-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-static-args-trailing-comma-multiple.js")]
    public Task test_cls_expr_private_meth_static_args_trailing_comma_multiple()
        => ExecutionTest("cls-expr-private-meth-static-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-static-args-trailing-comma-null.js")]
    public Task test_cls_expr_private_meth_static_args_trailing_comma_null()
        => ExecutionTest("cls-expr-private-meth-static-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-static-args-trailing-comma-single-args.js")]
    public Task test_cls_expr_private_meth_static_args_trailing_comma_single_args()
        => ExecutionTest("cls-expr-private-meth-static-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-static-args-trailing-comma-spread-operator.js")]
    public Task test_cls_expr_private_meth_static_args_trailing_comma_spread_operator()
        => ExecutionTest("cls-expr-private-meth-static-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/cls-expr-private-meth-static-args-trailing-comma-undefined.js")]
    public Task test_cls_expr_private_meth_static_args_trailing_comma_undefined()
        => ExecutionTest("cls-expr-private-meth-static-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/func-decl-args-trailing-comma-multiple.js")]
    public Task test_func_decl_args_trailing_comma_multiple()
        => ExecutionTest("func-decl-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/func-decl-args-trailing-comma-null.js")]
    public Task test_func_decl_args_trailing_comma_null()
        => ExecutionTest("func-decl-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/func-decl-args-trailing-comma-single-args.js")]
    public Task test_func_decl_args_trailing_comma_single_args()
        => ExecutionTest("func-decl-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/func-decl-args-trailing-comma-spread-operator.js")]
    public Task test_func_decl_args_trailing_comma_spread_operator()
        => ExecutionTest("func-decl-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/func-decl-args-trailing-comma-undefined.js")]
    public Task test_func_decl_args_trailing_comma_undefined()
        => ExecutionTest("func-decl-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/func-expr-args-trailing-comma-multiple.js")]
    public Task test_func_expr_args_trailing_comma_multiple()
        => ExecutionTest("func-expr-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/func-expr-args-trailing-comma-null.js")]
    public Task test_func_expr_args_trailing_comma_null()
        => ExecutionTest("func-expr-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/func-expr-args-trailing-comma-single-args.js")]
    public Task test_func_expr_args_trailing_comma_single_args()
        => ExecutionTest("func-expr-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/func-expr-args-trailing-comma-spread-operator.js")]
    public Task test_func_expr_args_trailing_comma_spread_operator()
        => ExecutionTest("func-expr-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/func-expr-args-trailing-comma-undefined.js")]
    public Task test_func_expr_args_trailing_comma_undefined()
        => ExecutionTest("func-expr-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/gen-func-decl-args-trailing-comma-multiple.js")]
    public Task test_gen_func_decl_args_trailing_comma_multiple()
        => ExecutionTest("gen-func-decl-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/gen-func-decl-args-trailing-comma-null.js")]
    public Task test_gen_func_decl_args_trailing_comma_null()
        => ExecutionTest("gen-func-decl-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/gen-func-decl-args-trailing-comma-single-args.js")]
    public Task test_gen_func_decl_args_trailing_comma_single_args()
        => ExecutionTest("gen-func-decl-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/gen-func-decl-args-trailing-comma-spread-operator.js")]
    public Task test_gen_func_decl_args_trailing_comma_spread_operator()
        => ExecutionTest("gen-func-decl-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/gen-func-decl-args-trailing-comma-undefined.js")]
    public Task test_gen_func_decl_args_trailing_comma_undefined()
        => ExecutionTest("gen-func-decl-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/gen-func-expr-args-trailing-comma-multiple.js")]
    public Task test_gen_func_expr_args_trailing_comma_multiple()
        => ExecutionTest("gen-func-expr-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/gen-func-expr-args-trailing-comma-null.js")]
    public Task test_gen_func_expr_args_trailing_comma_null()
        => ExecutionTest("gen-func-expr-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/gen-func-expr-args-trailing-comma-single-args.js")]
    public Task test_gen_func_expr_args_trailing_comma_single_args()
        => ExecutionTest("gen-func-expr-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/gen-func-expr-args-trailing-comma-spread-operator.js")]
    public Task test_gen_func_expr_args_trailing_comma_spread_operator()
        => ExecutionTest("gen-func-expr-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/gen-func-expr-args-trailing-comma-undefined.js")]
    public Task test_gen_func_expr_args_trailing_comma_undefined()
        => ExecutionTest("gen-func-expr-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/gen-meth-args-trailing-comma-multiple.js")]
    public Task test_gen_meth_args_trailing_comma_multiple()
        => ExecutionTest("gen-meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/gen-meth-args-trailing-comma-null.js")]
    public Task test_gen_meth_args_trailing_comma_null()
        => ExecutionTest("gen-meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/gen-meth-args-trailing-comma-single-args.js")]
    public Task test_gen_meth_args_trailing_comma_single_args()
        => ExecutionTest("gen-meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/gen-meth-args-trailing-comma-spread-operator.js")]
    public Task test_gen_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("gen-meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/gen-meth-args-trailing-comma-undefined.js")]
    public Task test_gen_meth_args_trailing_comma_undefined()
        => ExecutionTest("gen-meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-1.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_1()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-1");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-2.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_2()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-2");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-3.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_3()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-3");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-4.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_4()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-4");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-delete-1.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_delete_1()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-delete-1");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-delete-2.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_delete_2()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-delete-2");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-delete-3.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_delete_3()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-delete-3");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-delete-4.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_delete_4()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-delete-4");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-strict-delete-1.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_strict_delete_1()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-strict-delete-1");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-strict-delete-2.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_strict_delete_2()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-strict-delete-2");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-strict-delete-3.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_strict_delete_3()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-strict-delete-3");

    [Fact(DisplayName = "language/arguments-object/mapped/mapped-arguments-nonconfigurable-strict-delete-4.js")]
    public Task test_mapped_mapped_arguments_nonconfigurable_strict_delete_4()
        => ExecutionTest("mapped/mapped-arguments-nonconfigurable-strict-delete-4");

    [Fact(DisplayName = "language/arguments-object/mapped/nonconfigurable-descriptors-basic.js")]
    public Task test_mapped_nonconfigurable_descriptors_basic()
        => ExecutionTest("mapped/nonconfigurable-descriptors-basic");

    [Fact(DisplayName = "language/arguments-object/mapped/nonconfigurable-descriptors-define-failure.js")]
    public Task test_mapped_nonconfigurable_descriptors_define_failure()
        => ExecutionTest("mapped/nonconfigurable-descriptors-define-failure");

    [Fact(DisplayName = "language/arguments-object/mapped/nonconfigurable-descriptors-set-value-by-arguments.js")]
    public Task test_mapped_nonconfigurable_descriptors_set_value_by_arguments()
        => ExecutionTest("mapped/nonconfigurable-descriptors-set-value-by-arguments");

    [Fact(DisplayName = "language/arguments-object/mapped/nonconfigurable-descriptors-set-value-with-define-property.js")]
    public Task test_mapped_nonconfigurable_descriptors_set_value_with_define_property()
        => ExecutionTest("mapped/nonconfigurable-descriptors-set-value-with-define-property");

    [Fact(DisplayName = "language/arguments-object/mapped/nonwritable-nonconfigurable-descriptors-set-by-param.js")]
    public Task test_mapped_nonwritable_nonconfigurable_descriptors_set_by_param()
        => ExecutionTest("mapped/nonwritable-nonconfigurable-descriptors-set-by-param");

    [Fact(DisplayName = "language/arguments-object/mapped/nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-define-property.js")]
    public Task test_mapped_nonwritable_nonenumerable_nonconfigurable_descriptors_set_by_define_property()
        => ExecutionTest("mapped/nonwritable-nonenumerable-nonconfigurable-descriptors-set-by-define-property");

    [Fact(DisplayName = "language/arguments-object/mapped/writable-enumerable-configurable-descriptor.js")]
    public Task test_mapped_writable_enumerable_configurable_descriptor()
        => ExecutionTest("mapped/writable-enumerable-configurable-descriptor");

    [Fact(DisplayName = "language/arguments-object/meth-args-trailing-comma-multiple.js")]
    public Task test_meth_args_trailing_comma_multiple()
        => ExecutionTest("meth-args-trailing-comma-multiple");

    [Fact(DisplayName = "language/arguments-object/meth-args-trailing-comma-null.js")]
    public Task test_meth_args_trailing_comma_null()
        => ExecutionTest("meth-args-trailing-comma-null");

    [Fact(DisplayName = "language/arguments-object/meth-args-trailing-comma-single-args.js")]
    public Task test_meth_args_trailing_comma_single_args()
        => ExecutionTest("meth-args-trailing-comma-single-args");

    [Fact(DisplayName = "language/arguments-object/meth-args-trailing-comma-spread-operator.js")]
    public Task test_meth_args_trailing_comma_spread_operator()
        => ExecutionTest("meth-args-trailing-comma-spread-operator");

    [Fact(DisplayName = "language/arguments-object/meth-args-trailing-comma-undefined.js")]
    public Task test_meth_args_trailing_comma_undefined()
        => ExecutionTest("meth-args-trailing-comma-undefined");

    [Fact(DisplayName = "language/arguments-object/non-strict-arguments-object-is-immutable.js")]
    public Task test_non_strict_arguments_object_is_immutable()
        => ExecutionTest("non-strict-arguments-object-is-immutable");

    [Fact(DisplayName = "language/arguments-object/unmapped/via-params-dflt.js")]
    public Task test_unmapped_via_params_dflt()
        => ExecutionTest("unmapped/via-params-dflt");

    [Fact(DisplayName = "language/arguments-object/unmapped/via-params-dstr.js")]
    public Task test_unmapped_via_params_dstr()
        => ExecutionTest("unmapped/via-params-dstr");

    [Fact(DisplayName = "language/arguments-object/unmapped/via-params-rest.js")]
    public Task test_unmapped_via_params_rest()
        => ExecutionTest("unmapped/via-params-rest");

    [Fact(DisplayName = "language/arguments-object/unmapped/via-strict.js")]
    public Task test_unmapped_via_strict()
        => ExecutionTest("unmapped/via-strict");
}
