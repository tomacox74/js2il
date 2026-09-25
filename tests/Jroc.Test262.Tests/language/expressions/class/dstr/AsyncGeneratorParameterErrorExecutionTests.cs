using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.dstr;

public sealed class AsyncGeneratorParameterErrorExecutionTests : DiskExecutionTestsBase
{
    public AsyncGeneratorParameterErrorExecutionTests() : base("language.expressions.class_.dstr") { }

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-get-err.js")]
    public Task Ported_async_gen_meth_ary_init_iter_get_err() => ExecutionTestFromFile("async-gen-meth-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-throws.js")]
    public Task Ported_async_gen_meth_ary_ptrn_elem_id_init_throws() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_ary_ptrn_elem_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-step-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_elem_id_iter_step_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-val-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_elem_id_iter_val_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elision-step-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_elision_step_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-elision-next-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_rest_id_elision_next_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-iter-step-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_rest_id_iter_step_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-iter-val-err.js")]
    public Task Ported_async_gen_meth_ary_ptrn_rest_id_iter_val_err() => ExecutionTestFromFile("async-gen-meth-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-get-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_init_iter_get_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elision-step-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_elision_step_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task Ported_async_gen_meth_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTestFromFile("async-gen-meth-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-get-value-err.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_id_get_value_err() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-throws.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_id_init_throws() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-list-err.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_list_err() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-eval-err.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_prop_eval_err() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-get-value-err.js")]
    public Task Ported_async_gen_meth_obj_ptrn_id_get_value_err() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-throws.js")]
    public Task Ported_async_gen_meth_obj_ptrn_id_init_throws() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_obj_ptrn_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-list-err.js")]
    public Task Ported_async_gen_meth_obj_ptrn_list_err() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-eval-err.js")]
    public Task Ported_async_gen_meth_obj_ptrn_prop_eval_err() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-get-value-err.js")]
    public Task Ported_async_gen_meth_obj_ptrn_prop_id_get_value_err() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init-throws.js")]
    public Task Ported_async_gen_meth_obj_ptrn_prop_id_init_throws() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task Ported_async_gen_meth_obj_ptrn_prop_id_init_unresolvable() => ExecutionTestFromFile("async-gen-meth-obj-ptrn-prop-id-init-unresolvable");

}
