using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.try_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\try", "language.statements.try_") { }

    [Fact(DisplayName = "12.14-10")]
    public Task _12_14_10()
        => ExecutionTest("12.14-10");

    [Fact(DisplayName = "12.14-11")]
    public Task _12_14_11()
        => ExecutionTest("12.14-11");

    [Fact(DisplayName = "12.14-12")]
    public Task _12_14_12()
        => ExecutionTest("12.14-12");

    [Fact(DisplayName = "12.14-3")]
    public Task _12_14_3()
        => ExecutionTest("12.14-3");

    [Fact(DisplayName = "12.14-4")]
    public Task _12_14_4()
        => ExecutionTest("12.14-4");

    [Fact(DisplayName = "12.14-6")]
    public Task _12_14_6()
        => ExecutionTest("12.14-6");

    [Fact(DisplayName = "12.14-8")]
    public Task _12_14_8()
        => ExecutionTest("12.14-8");

    [Fact(DisplayName = "12.14-9")]
    public Task _12_14_9()
        => ExecutionTest("12.14-9");
    [Fact(DisplayName = "12.14-13")]
    public Task _12_14_13()
        => ExecutionTest("12.14-13");

    [Fact(DisplayName = "12.14-14")]
    public Task _12_14_14()
        => ExecutionTest("12.14-14");

    [Fact(DisplayName = "12.14-15")]
    public Task _12_14_15()
        => ExecutionTest("12.14-15");

    [Fact(DisplayName = "12.14-16")]
    public Task _12_14_16()
        => ExecutionTest("12.14-16");

    [Fact(DisplayName = "completion-values-fn-finally-return")]
    public Task completion_values_fn_finally_return()
        => ExecutionTest("completion-values-fn-finally-return");

    [Fact(DisplayName = "ary-init-iter-close")]
    public Task dstr_ary_init_iter_close()
        => ExecutionTest(@"dstr\ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype", Skip = "Blocked by shared Array.prototype iterator fallback semantics.")]
    public Task dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest(@"dstr\ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-no-close")]
    public Task dstr_ary_init_iter_no_close()
        => ExecutionTest(@"dstr\ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest(@"dstr\ary-name-iter-val");



    [Fact(DisplayName = "S12.14_A1")]
    public Task S12_14_A1()
        => ExecutionTest("S12.14_A1");

    [Fact(DisplayName = "S12.14_A2")]
    public Task S12_14_A2()
        => ExecutionTest("S12.14_A2");

    [Fact(DisplayName = "S12.14_A3")]
    public Task S12_14_A3()
        => ExecutionTest("S12.14_A3");

    [Fact(DisplayName = "S12.14_A4")]
    public Task S12_14_A4()
        => ExecutionTest("S12.14_A4");

    [Fact(DisplayName = "S12.14_A5")]
    public Task S12_14_A5()
        => ExecutionTest("S12.14_A5");

    [Fact(DisplayName = "S12.14_A6")]
    public Task S12_14_A6()
        => ExecutionTest("S12.14_A6");

    [Fact(DisplayName = "S12.14_A7_T1", Skip = "Currently triggers invalid IL for this nested try/finally control-flow shape.")]
    public Task S12_14_A7_T1()
        => ExecutionTest("S12.14_A7_T1");

    [Fact(DisplayName = "S12.14_A7_T2", Skip = "Currently triggers invalid IL for this nested try/finally control-flow shape.")]
    public Task S12_14_A7_T2()
        => ExecutionTest("S12.14_A7_T2");

    [Fact(DisplayName = "S12.14_A8")]
    public Task S12_14_A8()
        => ExecutionTest("S12.14_A8");
}
