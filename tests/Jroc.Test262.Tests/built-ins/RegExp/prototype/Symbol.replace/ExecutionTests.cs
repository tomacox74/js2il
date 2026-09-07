using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.replace;

public class ExecutionTests : DiskExecutionTestsBase
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

}
