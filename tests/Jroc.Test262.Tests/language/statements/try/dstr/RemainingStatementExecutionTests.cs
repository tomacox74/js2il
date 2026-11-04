namespace Jroc.Test262.Tests.language.statements.try_.dstr;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.try_.dstr") { }

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init.js")]
    public Task test_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws.js")]
    public Task test_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-class.js")]
    public Task test_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "obj-ptrn-id-init-throws.js")]
    public Task test_obj_ptrn_id_init_throws()
        => ExecutionTest("obj-ptrn-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-list-err.js")]
    public Task test_obj_ptrn_list_err()
        => ExecutionTest("obj-ptrn-list-err");

    [Fact(DisplayName = "obj-ptrn-prop-eval-err.js")]
    public Task test_obj_ptrn_prop_eval_err()
        => ExecutionTest("obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-get-value-err.js")]
    public Task test_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-throws.js")]
    public Task test_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("obj-ptrn-prop-id-init-throws");

}
