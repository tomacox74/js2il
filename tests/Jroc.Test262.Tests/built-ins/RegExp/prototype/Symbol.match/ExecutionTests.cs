using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.match;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.Symbol.match") { }

    [Fact(DisplayName = "builtin-coerce-lastindex.js")]
    public Task builtin_coerce_lastindex()
        => ExecutionTestFromFile("builtin-coerce-lastindex");

    [Fact(DisplayName = "builtin-failure-g-set-lastindex-err.js")]
    public Task builtin_failure_g_set_lastindex_err()
        => ExecutionTestFromFile("builtin-failure-g-set-lastindex-err");

    [Fact(DisplayName = "builtin-failure-g-set-lastindex.js")]
    public Task builtin_failure_g_set_lastindex()
        => ExecutionTestFromFile("builtin-failure-g-set-lastindex");

    [Fact(DisplayName = "builtin-failure-return-val.js")]
    public Task builtin_failure_return_val()
        => ExecutionTestFromFile("builtin-failure-return-val");

    [Fact(DisplayName = "builtin-failure-y-return-val.js")]
    public Task builtin_failure_y_return_val()
        => ExecutionTestFromFile("builtin-failure-y-return-val");

    [Fact(DisplayName = "builtin-failure-y-set-lastindex-err.js")]
    public Task builtin_failure_y_set_lastindex_err()
        => ExecutionTestFromFile("builtin-failure-y-set-lastindex-err");

    [Fact(DisplayName = "builtin-failure-y-set-lastindex.js")]
    public Task builtin_failure_y_set_lastindex()
        => ExecutionTestFromFile("builtin-failure-y-set-lastindex");

    [Fact(DisplayName = "builtin-infer-unicode.js")]
    public Task builtin_infer_unicode()
        => ExecutionTestFromFile("builtin-infer-unicode");

    [Fact(DisplayName = "builtin-success-return-val-groups.js")]
    public Task builtin_success_return_val_groups()
        => ExecutionTestFromFile("builtin-success-return-val-groups");

    [Fact(DisplayName = "builtin-success-return-val.js")]
    public Task builtin_success_return_val()
        => ExecutionTestFromFile("builtin-success-return-val");

    [Fact(DisplayName = "builtin-success-u-return-val-groups.js")]
    public Task builtin_success_u_return_val_groups()
        => ExecutionTestFromFile("builtin-success-u-return-val-groups");

    [Fact(DisplayName = "builtin-success-y-set-lastindex-err.js")]
    public Task builtin_success_y_set_lastindex_err()
        => ExecutionTestFromFile("builtin-success-y-set-lastindex-err");

    [Fact(DisplayName = "builtin-success-y-set-lastindex.js")]
    public Task builtin_success_y_set_lastindex()
        => ExecutionTestFromFile("builtin-success-y-set-lastindex");

    [Fact(DisplayName = "builtin-y-coerce-lastindex-err.js")]
    public Task builtin_y_coerce_lastindex_err()
        => ExecutionTestFromFile("builtin-y-coerce-lastindex-err");

    [Fact(DisplayName = "coerce-arg-err.js")]
    public Task coerce_arg_err()
        => ExecutionTestFromFile("coerce-arg-err");

    [Fact(DisplayName = "coerce-arg.js")]
    public Task coerce_arg()
        => ExecutionTestFromFile("coerce-arg");

    [Fact(DisplayName = "g-init-lastindex-err.js")]
    public Task g_init_lastindex_err()
        => ExecutionTestFromFile("g-init-lastindex-err");

    [Fact(DisplayName = "g-init-lastindex.js")]
    public Task g_init_lastindex()
        => ExecutionTestFromFile("g-init-lastindex");

    [Fact(DisplayName = "g-match-empty-advance-lastindex.js")]
    public Task g_match_empty_advance_lastindex()
        => ExecutionTestFromFile("g-match-empty-advance-lastindex");

    [Fact(DisplayName = "g-match-no-coerce-lastindex.js")]
    public Task g_match_no_coerce_lastindex()
        => ExecutionTestFromFile("g-match-no-coerce-lastindex");

    [Fact(DisplayName = "g-match-no-set-lastindex.js")]
    public Task g_match_no_set_lastindex()
        => ExecutionTestFromFile("g-match-no-set-lastindex");

    [Fact(DisplayName = "g-success-return-val.js")]
    public Task g_success_return_val()
        => ExecutionTestFromFile("g-success-return-val");

    [Fact(DisplayName = "g-zero-matches.js")]
    public Task g_zero_matches()
        => ExecutionTestFromFile("g-zero-matches");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "y-fail-global-return.js")]
    public Task y_fail_global_return()
        => ExecutionTestFromFile("y-fail-global-return");

    [Fact(DisplayName = "y-fail-lastindex-no-write.js")]
    public Task y_fail_lastindex_no_write()
        => ExecutionTestFromFile("y-fail-lastindex-no-write");

    [Fact(DisplayName = "y-fail-lastindex.js")]
    public Task y_fail_lastindex()
        => ExecutionTestFromFile("y-fail-lastindex");

    [Fact(DisplayName = "y-fail-return.js")]
    public Task y_fail_return()
        => ExecutionTestFromFile("y-fail-return");

    [Fact(DisplayName = "y-init-lastindex.js")]
    public Task y_init_lastindex()
        => ExecutionTestFromFile("y-init-lastindex");

    [Fact(DisplayName = "y-set-lastindex.js")]
    public Task y_set_lastindex()
        => ExecutionTestFromFile("y-set-lastindex");

}
