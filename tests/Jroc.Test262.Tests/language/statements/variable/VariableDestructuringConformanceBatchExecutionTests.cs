using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.variable;

public class VariableDestructuringConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public VariableDestructuringConformanceBatchExecutionTests()
        : base("language/statements/variable", "language.statements.variable") { }

    [Fact(DisplayName = "ary-init-iter-get-err.js")]
    public Task test_ary_init_iter_get_err()
        => ExecutionTest("dstr/ary-init-iter-get-err");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null.js")]
    public Task test_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole.js")]
    public Task test_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped.js")]
    public Task test_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws.js")]
    public Task test_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef.js")]
    public Task test_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete.js")]
    public Task test_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done.js")]
    public Task test_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-array-prototype");
}
