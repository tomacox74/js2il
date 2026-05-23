using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.const_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.const_") { }

    [Fact(DisplayName = "block-local-closure-get-before-initialization")]
    public Task block_local_closure_get_before_initialization()
        => ExecutionTest("block-local-closure-get-before-initialization");

    [Fact(DisplayName = "block-local-use-before-initialization-in-declaration-statement")]
    public Task block_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("block-local-use-before-initialization-in-declaration-statement");

    [Fact(DisplayName = "block-local-use-before-initialization-in-prior-statement")]
    public Task block_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("block-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "cptn-value", Skip = "Uses eval, which JS2IL does not support yet.")]
    public Task cptn_value()
        => ExecutionTest("cptn-value");

    [Fact(DisplayName = "fn-name-arrow")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");
    [Fact(DisplayName = "ary-init-iter-close", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_init_iter_close()
        => ExecutionTest(@"dstr\ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest(@"dstr\ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-no-close", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_init_iter_no_close()
        => ExecutionTest(@"dstr\ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest(@"dstr\ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-rest-iter");

}
