namespace Jroc.Test262.Tests.language.expressions.call;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "S11.2.3_A1.js", Skip = "eval is not supported.")]
    public Task S11_2_3_A1()
        => ExecutionTest("S11.2.3_A1");

    [Fact(DisplayName = "S11.2.3_A3_T4.js")]
    public Task S11_2_3_A3_T4()
        => ExecutionTest("S11.2.3_A3_T4");

    [Fact(DisplayName = "eval-err-args.js", Skip = "eval is not supported.")]
    public Task eval_err_args()
        => ExecutionTest("eval-err-args");

    [Fact(DisplayName = "eval-first-arg.js", Skip = "eval is not supported.")]
    public Task eval_first_arg()
        => ExecutionTest("eval-first-arg");

    [Fact(DisplayName = "eval-no-args.js", Skip = "eval is not supported.")]
    public Task eval_no_args()
        => ExecutionTest("eval-no-args");

    [Fact(DisplayName = "eval-realm-indirect.js", Skip = "eval is not supported.")]
    public Task eval_realm_indirect()
        => ExecutionTest("eval-realm-indirect");

    [Fact(DisplayName = "eval-spread-empty-leading.js", Skip = "eval is not supported.")]
    public Task eval_spread_empty_leading()
        => ExecutionTest("eval-spread-empty-leading");

    [Fact(DisplayName = "eval-spread-empty-trailing.js", Skip = "eval is not supported.")]
    public Task eval_spread_empty_trailing()
        => ExecutionTest("eval-spread-empty-trailing");

    [Fact(DisplayName = "eval-spread-empty.js", Skip = "eval is not supported.")]
    public Task eval_spread_empty()
        => ExecutionTest("eval-spread-empty");

    [Fact(DisplayName = "eval-spread.js", Skip = "eval is not supported.")]
    public Task eval_spread()
        => ExecutionTest("eval-spread");

    [Fact(DisplayName = "eval-strictness-inherit-non-strict.js", Skip = "eval is not supported.")]
    public Task eval_strictness_inherit_non_strict()
        => ExecutionTest("eval-strictness-inherit-non-strict");

    [Fact(DisplayName = "eval-strictness-inherit-strict.js", Skip = "eval is not supported.")]
    public Task eval_strictness_inherit_strict()
        => ExecutionTest("eval-strictness-inherit-strict");

    [Fact(DisplayName = "with-base-obj.js")]
    public Task with_base_obj()
        => ExecutionTest("with-base-obj");
}
