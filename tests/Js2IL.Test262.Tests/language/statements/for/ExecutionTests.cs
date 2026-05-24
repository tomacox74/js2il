using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_") { }

    [Fact(DisplayName = "12.6.3_2-3-a-ii-1")]
    public Task _12_6_3_2_3_a_ii_1()
        => ExecutionTest("12.6.3_2-3-a-ii-1");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-10")]
    public Task _12_6_3_2_3_a_ii_10()
        => ExecutionTest("12.6.3_2-3-a-ii-10");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-11")]
    public Task _12_6_3_2_3_a_ii_11()
        => ExecutionTest("12.6.3_2-3-a-ii-11");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-12")]
    public Task _12_6_3_2_3_a_ii_12()
        => ExecutionTest("12.6.3_2-3-a-ii-12");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-13")]
    public Task _12_6_3_2_3_a_ii_13()
        => ExecutionTest("12.6.3_2-3-a-ii-13");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-14")]
    public Task _12_6_3_2_3_a_ii_14()
        => ExecutionTest("12.6.3_2-3-a-ii-14");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-15")]
    public Task _12_6_3_2_3_a_ii_15()
        => ExecutionTest("12.6.3_2-3-a-ii-15");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-16")]
    public Task _12_6_3_2_3_a_ii_16()
        => ExecutionTest("12.6.3_2-3-a-ii-16");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-17")]
    public Task _12_6_3_2_3_a_ii_17()
        => ExecutionTest("12.6.3_2-3-a-ii-17");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-18")]
    public Task _12_6_3_2_3_a_ii_18()
        => ExecutionTest("12.6.3_2-3-a-ii-18");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-19")]
    public Task _12_6_3_2_3_a_ii_19()
        => ExecutionTest("12.6.3_2-3-a-ii-19");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-2")]
    public Task _12_6_3_2_3_a_ii_2()
        => ExecutionTest("12.6.3_2-3-a-ii-2");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-20")]
    public Task _12_6_3_2_3_a_ii_20()
        => ExecutionTest("12.6.3_2-3-a-ii-20");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-21")]
    public Task _12_6_3_2_3_a_ii_21()
        => ExecutionTest("12.6.3_2-3-a-ii-21");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-3")]
    public Task _12_6_3_2_3_a_ii_3()
        => ExecutionTest("12.6.3_2-3-a-ii-3");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-4")]
    public Task _12_6_3_2_3_a_ii_4()
        => ExecutionTest("12.6.3_2-3-a-ii-4");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-5")]
    public Task _12_6_3_2_3_a_ii_5()
        => ExecutionTest("12.6.3_2-3-a-ii-5");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-6")]
    public Task _12_6_3_2_3_a_ii_6()
        => ExecutionTest("12.6.3_2-3-a-ii-6");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-7")]
    public Task _12_6_3_2_3_a_ii_7()
        => ExecutionTest("12.6.3_2-3-a-ii-7");
    [Fact(DisplayName = "12.6.3_2-3-a-ii-8")]
    public Task _12_6_3_2_3_a_ii_8()
        => ExecutionTest("12.6.3_2-3-a-ii-8");

    [Fact(DisplayName = "12.6.3_2-3-a-ii-9")]
    public Task _12_6_3_2_3_a_ii_9()
        => ExecutionTest("12.6.3_2-3-a-ii-9");

    [Fact(DisplayName = "const-ary-init-iter-close")]
    public Task dstr_const_ary_init_iter_close()
        => ExecutionTest(@"dstr\const-ary-init-iter-close");

    [Fact(DisplayName = "const-ary-init-iter-get-err-array-prototype", Skip = "Blocked by shared Array.prototype iterator fallback semantics.")]
    public Task dstr_const_ary_init_iter_get_err_array_prototype()
        => ExecutionTest(@"dstr\const-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "const-ary-init-iter-no-close")]
    public Task dstr_const_ary_init_iter_no_close()
        => ExecutionTest(@"dstr\const-ary-init-iter-no-close");

    [Fact(DisplayName = "const-ary-name-iter-val")]
    public Task dstr_const_ary_name_iter_val()
        => ExecutionTest(@"dstr\const-ary-name-iter-val");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elem-init")]
    public Task dstr_const_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest(@"dstr\const-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elem-iter")]
    public Task dstr_const_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest(@"dstr\const-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elision-init", Skip = "Blocked by existing generator scope capture issue.")]
    public Task dstr_const_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest(@"dstr\const-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elision-iter")]
    public Task dstr_const_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest(@"dstr\const-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-empty-init")]
    public Task dstr_const_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest(@"dstr\const-ary-ptrn-elem-ary-empty-init");

}
