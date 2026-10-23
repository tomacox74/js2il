namespace Jroc.Test262.Tests.language.expressions.yield_;

public sealed class YieldCompletionExecutionTests : DiskExecutionTestsBase
{
    public YieldCompletionExecutionTests() : base("language.expressions.yield") { }

    [Fact(DisplayName = "rhs-unresolvable.js")]
    public Task rhs_unresolvable()
        => ExecutionTest("rhs-unresolvable");

    [Fact(DisplayName = "star-return-is-null.js")]
    public Task star_return_is_null()
        => ExecutionTest("star-return-is-null");

    [Fact(DisplayName = "star-rhs-iter-get-call-err.js")]
    public Task star_rhs_iter_get_call_err()
        => ExecutionTest("star-rhs-iter-get-call-err");

    [Fact(DisplayName = "star-rhs-iter-get-call-non-obj.js")]
    public Task star_rhs_iter_get_call_non_obj()
        => ExecutionTest("star-rhs-iter-get-call-non-obj");

    [Fact(DisplayName = "star-rhs-iter-get-get-err.js")]
    public Task star_rhs_iter_get_get_err()
        => ExecutionTest("star-rhs-iter-get-get-err");

    [Fact(DisplayName = "star-rhs-iter-nrml-next-call-err.js")]
    public Task star_rhs_iter_nrml_next_call_err()
        => ExecutionTest("star-rhs-iter-nrml-next-call-err");

    [Fact(DisplayName = "star-rhs-iter-nrml-next-call-non-obj.js")]
    public Task star_rhs_iter_nrml_next_call_non_obj()
        => ExecutionTest("star-rhs-iter-nrml-next-call-non-obj");

    [Fact(DisplayName = "star-rhs-iter-nrml-next-get-err.js")]
    public Task star_rhs_iter_nrml_next_get_err()
        => ExecutionTest("star-rhs-iter-nrml-next-get-err");

    [Fact(DisplayName = "star-rhs-iter-nrml-next-invoke.js")]
    public Task star_rhs_iter_nrml_next_invoke()
        => ExecutionTest("star-rhs-iter-nrml-next-invoke");

    [Fact(DisplayName = "star-rhs-iter-nrml-res-done-err.js")]
    public Task star_rhs_iter_nrml_res_done_err()
        => ExecutionTest("star-rhs-iter-nrml-res-done-err");

    [Fact(DisplayName = "star-rhs-iter-nrml-res-value-err.js")]
    public Task star_rhs_iter_nrml_res_value_err()
        => ExecutionTest("star-rhs-iter-nrml-res-value-err");

    [Fact(DisplayName = "star-rhs-iter-nrml-res-value-final.js")]
    public Task star_rhs_iter_nrml_res_value_final()
        => ExecutionTest("star-rhs-iter-nrml-res-value-final");

    [Fact(DisplayName = "star-rhs-iter-rtrn-no-rtrn.js")]
    public Task star_rhs_iter_rtrn_no_rtrn()
        => ExecutionTest("star-rhs-iter-rtrn-no-rtrn");

    [Fact(DisplayName = "star-rhs-iter-rtrn-res-done-err.js")]
    public Task star_rhs_iter_rtrn_res_done_err()
        => ExecutionTest("star-rhs-iter-rtrn-res-done-err");

    [Fact(DisplayName = "star-rhs-iter-rtrn-res-done-no-value.js")]
    public Task star_rhs_iter_rtrn_res_done_no_value()
        => ExecutionTest("star-rhs-iter-rtrn-res-done-no-value");

    [Fact(DisplayName = "star-rhs-iter-rtrn-res-value-err.js")]
    public Task star_rhs_iter_rtrn_res_value_err()
        => ExecutionTest("star-rhs-iter-rtrn-res-value-err");

    [Fact(DisplayName = "star-rhs-iter-rtrn-res-value-final.js")]
    public Task star_rhs_iter_rtrn_res_value_final()
        => ExecutionTest("star-rhs-iter-rtrn-res-value-final");

    [Fact(DisplayName = "star-rhs-iter-rtrn-rtrn-call-err.js")]
    public Task star_rhs_iter_rtrn_rtrn_call_err()
        => ExecutionTest("star-rhs-iter-rtrn-rtrn-call-err");

    [Fact(DisplayName = "star-rhs-iter-rtrn-rtrn-call-non-obj.js")]
    public Task star_rhs_iter_rtrn_rtrn_call_non_obj()
        => ExecutionTest("star-rhs-iter-rtrn-rtrn-call-non-obj");

    [Fact(DisplayName = "star-rhs-iter-rtrn-rtrn-get-err.js")]
    public Task star_rhs_iter_rtrn_rtrn_get_err()
        => ExecutionTest("star-rhs-iter-rtrn-rtrn-get-err");

    [Fact(DisplayName = "star-rhs-iter-rtrn-rtrn-invoke.js")]
    public Task star_rhs_iter_rtrn_rtrn_invoke()
        => ExecutionTest("star-rhs-iter-rtrn-rtrn-invoke");

    [Fact(DisplayName = "star-rhs-iter-thrw-res-done-err.js")]
    public Task star_rhs_iter_thrw_res_done_err()
        => ExecutionTest("star-rhs-iter-thrw-res-done-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-res-done-no-value.js")]
    public Task star_rhs_iter_thrw_res_done_no_value()
        => ExecutionTest("star-rhs-iter-thrw-res-done-no-value");

    [Fact(DisplayName = "star-rhs-iter-thrw-res-value-err.js")]
    public Task star_rhs_iter_thrw_res_value_err()
        => ExecutionTest("star-rhs-iter-thrw-res-value-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-res-value-final.js")]
    public Task star_rhs_iter_thrw_res_value_final()
        => ExecutionTest("star-rhs-iter-thrw-res-value-final");

    [Fact(DisplayName = "star-rhs-iter-thrw-thrw-call-err.js")]
    public Task star_rhs_iter_thrw_thrw_call_err()
        => ExecutionTest("star-rhs-iter-thrw-thrw-call-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-thrw-call-non-obj.js")]
    public Task star_rhs_iter_thrw_thrw_call_non_obj()
        => ExecutionTest("star-rhs-iter-thrw-thrw-call-non-obj");

    [Fact(DisplayName = "star-rhs-iter-thrw-thrw-get-err.js")]
    public Task star_rhs_iter_thrw_thrw_get_err()
        => ExecutionTest("star-rhs-iter-thrw-thrw-get-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-thrw-invoke.js")]
    public Task star_rhs_iter_thrw_thrw_invoke()
        => ExecutionTest("star-rhs-iter-thrw-thrw-invoke");

    [Fact(DisplayName = "star-rhs-iter-thrw-violation-no-rtrn.js")]
    public Task star_rhs_iter_thrw_violation_no_rtrn()
        => ExecutionTest("star-rhs-iter-thrw-violation-no-rtrn");

    [Fact(DisplayName = "star-rhs-iter-thrw-violation-rtrn-call-err.js")]
    public Task star_rhs_iter_thrw_violation_rtrn_call_err()
        => ExecutionTest("star-rhs-iter-thrw-violation-rtrn-call-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-violation-rtrn-call-non-obj.js")]
    public Task star_rhs_iter_thrw_violation_rtrn_call_non_obj()
        => ExecutionTest("star-rhs-iter-thrw-violation-rtrn-call-non-obj");

    [Fact(DisplayName = "star-rhs-iter-thrw-violation-rtrn-get-err.js")]
    public Task star_rhs_iter_thrw_violation_rtrn_get_err()
        => ExecutionTest("star-rhs-iter-thrw-violation-rtrn-get-err");

    [Fact(DisplayName = "star-rhs-iter-thrw-violation-rtrn-invoke.js")]
    public Task star_rhs_iter_thrw_violation_rtrn_invoke()
        => ExecutionTest("star-rhs-iter-thrw-violation-rtrn-invoke");

    [Fact(DisplayName = "star-rhs-unresolvable.js")]
    public Task star_rhs_unresolvable()
        => ExecutionTest("star-rhs-unresolvable");

    [Fact(DisplayName = "star-throw-is-null.js")]
    public Task star_throw_is_null()
        => ExecutionTest("star-throw-is-null");
}
