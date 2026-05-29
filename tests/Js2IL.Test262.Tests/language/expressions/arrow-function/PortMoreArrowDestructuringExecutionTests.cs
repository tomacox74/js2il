using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.arrow_function;

public class PortMoreArrowDestructuringExecutionTests : DiskExecutionTestsBase
{
    public PortMoreArrowDestructuringExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init")]
    public Task dstr_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter")]
    public Task dstr_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter")]
    public Task dstr_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init")]
    public Task dstr_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter")]
    public Task dstr_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null")]
    public Task dstr_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted")]
    public Task dstr_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole")]
    public Task dstr_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped")]
    public Task dstr_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws")]
    public Task dstr_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef")]
    public Task dstr_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete")]
    public Task dstr_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done")]
    public Task dstr_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val")]
    public Task dstr_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id-init")]
    public Task dstr_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id")]
    public Task dstr_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id");

}
