using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.new_;

public class ConstructionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructionCompletionExecutionTests() : base("language.expressions.new") { }

    [Fact(DisplayName = "ctorExpr-fn-ref-before-args-eval-fn-wrapup.js")]
    public Task test_ctorExpr_fn_ref_before_args_eval_fn_wrapup() => ExecutionTest("ctorExpr-fn-ref-before-args-eval-fn-wrapup");

    [Fact(DisplayName = "ctorExpr-fn-ref-before-args-eval.js")]
    public Task test_ctorExpr_fn_ref_before_args_eval() => ExecutionTest("ctorExpr-fn-ref-before-args-eval");

    [Fact(DisplayName = "non-ctor-err-realm.js")]
    public Task test_non_ctor_err_realm() => ExecutionTest("non-ctor-err-realm");

    [Fact(DisplayName = "S11.2.2_A1.1.js", Skip = "eval is not supported.")]
    public Task test_S11_2_2_A1_1() => ExecutionTest("S11.2.2_A1.1");

    [Fact(DisplayName = "S11.2.2_A1.2.js", Skip = "eval is not supported.")]
    public Task test_S11_2_2_A1_2() => ExecutionTest("S11.2.2_A1.2");

    [Fact(DisplayName = "spread-err-mult-err-expr-throws.js")]
    public Task test_spread_err_mult_err_expr_throws() => ExecutionTest("spread-err-mult-err-expr-throws");

    [Fact(DisplayName = "spread-err-mult-err-iter-get-value.js")]
    public Task test_spread_err_mult_err_iter_get_value() => ExecutionTest("spread-err-mult-err-iter-get-value");

    [Fact(DisplayName = "spread-err-mult-err-itr-get-call.js")]
    public Task test_spread_err_mult_err_itr_get_call() => ExecutionTest("spread-err-mult-err-itr-get-call");

    [Fact(DisplayName = "spread-err-mult-err-itr-get-get.js")]
    public Task test_spread_err_mult_err_itr_get_get() => ExecutionTest("spread-err-mult-err-itr-get-get");

    [Fact(DisplayName = "spread-err-mult-err-itr-step.js")]
    public Task test_spread_err_mult_err_itr_step() => ExecutionTest("spread-err-mult-err-itr-step");

    [Fact(DisplayName = "spread-err-mult-err-itr-value.js")]
    public Task test_spread_err_mult_err_itr_value() => ExecutionTest("spread-err-mult-err-itr-value");

    [Fact(DisplayName = "spread-err-mult-err-unresolvable.js")]
    public Task test_spread_err_mult_err_unresolvable() => ExecutionTest("spread-err-mult-err-unresolvable");

    [Fact(DisplayName = "spread-err-sngl-err-expr-throws.js")]
    public Task test_spread_err_sngl_err_expr_throws() => ExecutionTest("spread-err-sngl-err-expr-throws");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-call.js")]
    public Task test_spread_err_sngl_err_itr_get_call() => ExecutionTest("spread-err-sngl-err-itr-get-call");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-get.js")]
    public Task test_spread_err_sngl_err_itr_get_get() => ExecutionTest("spread-err-sngl-err-itr-get-get");

    [Fact(DisplayName = "spread-err-sngl-err-itr-get-value.js")]
    public Task test_spread_err_sngl_err_itr_get_value() => ExecutionTest("spread-err-sngl-err-itr-get-value");

    [Fact(DisplayName = "spread-err-sngl-err-itr-step.js")]
    public Task test_spread_err_sngl_err_itr_step() => ExecutionTest("spread-err-sngl-err-itr-step");

    [Fact(DisplayName = "spread-err-sngl-err-itr-value.js")]
    public Task test_spread_err_sngl_err_itr_value() => ExecutionTest("spread-err-sngl-err-itr-value");

    [Fact(DisplayName = "spread-err-sngl-err-unresolvable.js")]
    public Task test_spread_err_sngl_err_unresolvable() => ExecutionTest("spread-err-sngl-err-unresolvable");

    [Fact(DisplayName = "spread-mult-empty.js")]
    public Task test_spread_mult_empty() => ExecutionTest("spread-mult-empty");

    [Fact(DisplayName = "spread-mult-expr.js")]
    public Task test_spread_mult_expr() => ExecutionTest("spread-mult-expr");

    [Fact(DisplayName = "spread-mult-iter.js")]
    public Task test_spread_mult_iter() => ExecutionTest("spread-mult-iter");

    [Fact(DisplayName = "spread-mult-literal.js")]
    public Task test_spread_mult_literal() => ExecutionTest("spread-mult-literal");

    [Fact(DisplayName = "spread-sngl-empty.js")]
    public Task test_spread_sngl_empty() => ExecutionTest("spread-sngl-empty");

    [Fact(DisplayName = "spread-sngl-expr.js")]
    public Task test_spread_sngl_expr() => ExecutionTest("spread-sngl-expr");

    [Fact(DisplayName = "spread-sngl-iter.js")]
    public Task test_spread_sngl_iter() => ExecutionTest("spread-sngl-iter");

    [Fact(DisplayName = "spread-sngl-literal.js")]
    public Task test_spread_sngl_literal() => ExecutionTest("spread-sngl-literal");
}
