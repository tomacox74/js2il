using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.replace;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.Symbol.replace") { }

    [Fact(DisplayName = "arg-1-coerce-err.js")]
    public Task arg_1_coerce_err()
        => ExecutionTestFromFile("arg-1-coerce-err");

    [Fact(DisplayName = "arg-1-coerce.js")]
    public Task arg_1_coerce()
        => ExecutionTestFromFile("arg-1-coerce");

    [Fact(DisplayName = "arg-2-coerce-err.js")]
    public Task arg_2_coerce_err()
        => ExecutionTestFromFile("arg-2-coerce-err");

    [Fact(DisplayName = "arg-2-coerce.js")]
    public Task arg_2_coerce()
        => ExecutionTestFromFile("arg-2-coerce");

    [Fact(DisplayName = "fn-coerce-replacement-err.js")]
    public Task fn_coerce_replacement_err()
        => ExecutionTestFromFile("fn-coerce-replacement-err");

    [Fact(DisplayName = "fn-coerce-replacement.js")]
    public Task fn_coerce_replacement()
        => ExecutionTestFromFile("fn-coerce-replacement");

    [Fact(DisplayName = "fn-err.js")]
    public Task fn_err()
        => ExecutionTestFromFile("fn-err");

    [Fact(DisplayName = "fn-invoke-args.js")]
    public Task fn_invoke_args()
        => ExecutionTestFromFile("fn-invoke-args");

    [Fact(DisplayName = "fn-invoke-this-no-strict.js")]
    public Task fn_invoke_this_no_strict()
        => ExecutionTestFromFile("fn-invoke-this-no-strict");

    [Fact(DisplayName = "fn-invoke-this-strict.js")]
    public Task fn_invoke_this_strict()
        => ExecutionTestFromFile("fn-invoke-this-strict");

    [Fact(DisplayName = "g-init-lastindex.js")]
    public Task g_init_lastindex()
        => ExecutionTestFromFile("g-init-lastindex");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "match-failure.js")]
    public Task match_failure()
        => ExecutionTestFromFile("match-failure");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "replace-with-trailing.js")]
    public Task replace_with_trailing()
        => ExecutionTestFromFile("replace-with-trailing");

    [Fact(DisplayName = "replace-without-trailing.js")]
    public Task replace_without_trailing()
        => ExecutionTestFromFile("replace-without-trailing");

    [Fact(DisplayName = "subst-after.js")]
    public Task subst_after()
        => ExecutionTestFromFile("subst-after");

    [Fact(DisplayName = "subst-before.js")]
    public Task subst_before()
        => ExecutionTestFromFile("subst-before");

    [Fact(DisplayName = "subst-capture-idx-1.js")]
    public Task subst_capture_idx_1()
        => ExecutionTestFromFile("subst-capture-idx-1");

    [Fact(DisplayName = "subst-capture-idx-2.js")]
    public Task subst_capture_idx_2()
        => ExecutionTestFromFile("subst-capture-idx-2");

    [Fact(DisplayName = "subst-dollar.js")]
    public Task subst_dollar()
        => ExecutionTestFromFile("subst-dollar");

    [Fact(DisplayName = "subst-matched.js")]
    public Task subst_matched()
        => ExecutionTestFromFile("subst-matched");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "coerce-lastindex-err.js")]
    public Task ported_coerce_lastindex_err() => ExecutionTestFromFile("coerce-lastindex-err");

    [Fact(DisplayName = "coerce-lastindex.js")]
    public Task ported_coerce_lastindex() => ExecutionTestFromFile("coerce-lastindex");

    [Fact(DisplayName = "coerce-unicode.js")]
    public Task ported_coerce_unicode() => ExecutionTestFromFile("coerce-unicode");

    [Fact(DisplayName = "exec-err.js")]
    public Task ported_exec_err() => ExecutionTestFromFile("exec-err");

    [Fact(DisplayName = "exec-invocation.js")]
    public Task ported_exec_invocation() => ExecutionTestFromFile("exec-invocation");

    [Fact(DisplayName = "flags-tostring-error.js")]
    public Task ported_flags_tostring_error() => ExecutionTestFromFile("flags-tostring-error");

    [Fact(DisplayName = "fn-invoke-args-empty-result.js")]
    public Task ported_fn_invoke_args_empty_result() => ExecutionTestFromFile("fn-invoke-args-empty-result");

    [Fact(DisplayName = "g-init-lastindex-err.js")]
    public Task ported_g_init_lastindex_err() => ExecutionTestFromFile("g-init-lastindex-err");

    [Fact(DisplayName = "g-pos-decrement.js")]
    public Task ported_g_pos_decrement() => ExecutionTestFromFile("g-pos-decrement");

    [Fact(DisplayName = "g-pos-increment.js")]
    public Task ported_g_pos_increment() => ExecutionTestFromFile("g-pos-increment");

    [Fact(DisplayName = "get-exec-err.js")]
    public Task ported_get_exec_err() => ExecutionTestFromFile("get-exec-err");

    [Fact(DisplayName = "get-flags-err.js")]
    public Task ported_get_flags_err() => ExecutionTestFromFile("get-flags-err");

    [Fact(DisplayName = "get-global-err.js")]
    public Task ported_get_global_err() => ExecutionTestFromFile("get-global-err");

    [Fact(DisplayName = "get-unicode-error.js")]
    public Task ported_get_unicode_error() => ExecutionTestFromFile("get-unicode-error");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "named-groups-fn.js")]
    public Task ported_named_groups_fn() => ExecutionTestFromFile("named-groups-fn");

    [Fact(DisplayName = "named-groups.js")]
    public Task ported_named_groups() => ExecutionTestFromFile("named-groups");

    [Fact(DisplayName = "poisoned-stdlib.js")]
    public Task ported_poisoned_stdlib() => ExecutionTestFromFile("poisoned-stdlib");

    [Fact(DisplayName = "result-coerce-capture-err.js")]
    public Task ported_result_coerce_capture_err() => ExecutionTestFromFile("result-coerce-capture-err");

    [Fact(DisplayName = "result-coerce-capture.js")]
    public Task ported_result_coerce_capture() => ExecutionTestFromFile("result-coerce-capture");

    [Fact(DisplayName = "result-coerce-groups-err.js")]
    public Task ported_result_coerce_groups_err() => ExecutionTestFromFile("result-coerce-groups-err");

    [Fact(DisplayName = "result-coerce-groups-prop-err.js")]
    public Task ported_result_coerce_groups_prop_err() => ExecutionTestFromFile("result-coerce-groups-prop-err");

    [Fact(DisplayName = "result-coerce-groups-prop.js")]
    public Task ported_result_coerce_groups_prop() => ExecutionTestFromFile("result-coerce-groups-prop");

    [Fact(DisplayName = "result-coerce-groups.js")]
    public Task ported_result_coerce_groups() => ExecutionTestFromFile("result-coerce-groups");

    [Fact(DisplayName = "result-coerce-index-err.js")]
    public Task ported_result_coerce_index_err() => ExecutionTestFromFile("result-coerce-index-err");

    [Fact(DisplayName = "result-coerce-index-undefined.js")]
    public Task ported_result_coerce_index_undefined() => ExecutionTestFromFile("result-coerce-index-undefined");

    [Fact(DisplayName = "result-coerce-index.js")]
    public Task ported_result_coerce_index() => ExecutionTestFromFile("result-coerce-index");

    [Fact(DisplayName = "result-coerce-length-err.js")]
    public Task ported_result_coerce_length_err() => ExecutionTestFromFile("result-coerce-length-err");

    [Fact(DisplayName = "result-coerce-length.js")]
    public Task ported_result_coerce_length() => ExecutionTestFromFile("result-coerce-length");

    [Fact(DisplayName = "result-coerce-matched-err.js")]
    public Task ported_result_coerce_matched_err() => ExecutionTestFromFile("result-coerce-matched-err");

    [Fact(DisplayName = "result-coerce-matched-global.js")]
    public Task ported_result_coerce_matched_global() => ExecutionTestFromFile("result-coerce-matched-global");

    [Fact(DisplayName = "result-coerce-matched.js")]
    public Task ported_result_coerce_matched() => ExecutionTestFromFile("result-coerce-matched");

    [Fact(DisplayName = "result-get-capture-err.js")]
    public Task ported_result_get_capture_err() => ExecutionTestFromFile("result-get-capture-err");

    [Fact(DisplayName = "result-get-groups-err.js")]
    public Task ported_result_get_groups_err() => ExecutionTestFromFile("result-get-groups-err");

    [Fact(DisplayName = "result-get-groups-prop-err.js")]
    public Task ported_result_get_groups_prop_err() => ExecutionTestFromFile("result-get-groups-prop-err");

    [Fact(DisplayName = "result-get-index-err.js")]
    public Task ported_result_get_index_err() => ExecutionTestFromFile("result-get-index-err");

    [Fact(DisplayName = "result-get-length-err.js")]
    public Task ported_result_get_length_err() => ExecutionTestFromFile("result-get-length-err");

    [Fact(DisplayName = "result-get-matched-err.js")]
    public Task ported_result_get_matched_err() => ExecutionTestFromFile("result-get-matched-err");

    [Fact(DisplayName = "u-advance-after-empty.js")]
    public Task ported_u_advance_after_empty() => ExecutionTestFromFile("u-advance-after-empty");

    [Fact(DisplayName = "y-fail-global-return.js")]
    public Task ported_y_fail_global_return() => ExecutionTestFromFile("y-fail-global-return");

    [Fact(DisplayName = "y-fail-lastindex-no-write.js")]
    public Task ported_y_fail_lastindex_no_write() => ExecutionTestFromFile("y-fail-lastindex-no-write");

    [Fact(DisplayName = "y-fail-lastindex.js")]
    public Task ported_y_fail_lastindex() => ExecutionTestFromFile("y-fail-lastindex");

    [Fact(DisplayName = "y-fail-return.js")]
    public Task ported_y_fail_return() => ExecutionTestFromFile("y-fail-return");

    [Fact(DisplayName = "y-init-lastindex.js")]
    public Task ported_y_init_lastindex() => ExecutionTestFromFile("y-init-lastindex");

    [Fact(DisplayName = "y-set-lastindex.js")]
    public Task ported_y_set_lastindex() => ExecutionTestFromFile("y-set-lastindex");
}
