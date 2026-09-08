using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.directive_prologue;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/directive-prologue", "language.directive_prologue") { }

    [Fact(DisplayName = "10.1.1-1-s.js")]
    public Task _10_1_1_1_s()
        => ExecutionTest("10.1.1-1-s");

    [Fact(DisplayName = "10.1.1-10-s.js")]
    public Task _10_1_1_10_s()
        => ExecutionTest("10.1.1-10-s");

    [Fact(DisplayName = "10.1.1-28-s.js")]
    public Task _10_1_1_28_s()
        => ExecutionTest("10.1.1-28-s");

    [Fact(DisplayName = "10.1.1-29-s.js")]
    public Task _10_1_1_29_s()
        => ExecutionTest("10.1.1-29-s");

    [Fact(DisplayName = "10.1.1-3-s.js")]
    public Task _10_1_1_3_s()
        => ExecutionTest("10.1.1-3-s");

    [Fact(DisplayName = "10.1.1-31-s.js")]
    public Task _10_1_1_31_s()
        => ExecutionTest("10.1.1-31-s");

    [Fact(DisplayName = "10.1.1-32-s.js")]
    public Task _10_1_1_32_s()
        => ExecutionTest("10.1.1-32-s");

    [Fact(DisplayName = "10.1.1-4-s.js")]
    public Task _10_1_1_4_s()
        => ExecutionTest("10.1.1-4-s");

    [Fact(DisplayName = "10.1.1-6-s.js")]
    public Task _10_1_1_6_s()
        => ExecutionTest("10.1.1-6-s");

    [Fact(DisplayName = "10.1.1-7-s.js")]
    public Task _10_1_1_7_s()
        => ExecutionTest("10.1.1-7-s");

    [Fact(DisplayName = "10.1.1-9-s.js")]
    public Task _10_1_1_9_s()
        => ExecutionTest("10.1.1-9-s");

    [Fact(DisplayName = "14.1-1-s.js")]
    public Task _14_1_1_s()
        => ExecutionTest("14.1-1-s");

    [Fact(DisplayName = "14.1-10-s.js")]
    public Task _14_1_10_s()
        => ExecutionTest("14.1-10-s");

    [Fact(DisplayName = "14.1-11-s.js")]
    public Task _14_1_11_s()
        => ExecutionTest("14.1-11-s");

    [Fact(DisplayName = "14.1-12-s.js")]
    public Task _14_1_12_s()
        => ExecutionTest("14.1-12-s");

    [Fact(DisplayName = "14.1-13-s.js")]
    public Task _14_1_13_s()
        => ExecutionTest("14.1-13-s");

    [Fact(DisplayName = "14.1-14-s.js")]
    public Task _14_1_14_s()
        => ExecutionTest("14.1-14-s");

    [Fact(DisplayName = "14.1-15-s.js")]
    public Task _14_1_15_s()
        => ExecutionTest("14.1-15-s");

    [Fact(DisplayName = "14.1-16-s.js")]
    public Task _14_1_16_s()
        => ExecutionTest("14.1-16-s");

    [Fact(DisplayName = "14.1-17-s.js")]
    public Task _14_1_17_s()
        => ExecutionTest("14.1-17-s");

    [Fact(DisplayName = "14.1-2-s.js")]
    public Task _14_1_2_s()
        => ExecutionTest("14.1-2-s");

    [Fact(DisplayName = "14.1-3-s.js")]
    public Task _14_1_3_s()
        => ExecutionTest("14.1-3-s");

    [Fact(DisplayName = "14.1-4-s.js")]
    public Task _14_1_4_s()
        => ExecutionTest("14.1-4-s");

    [Fact(DisplayName = "14.1-5-s.js")]
    public Task _14_1_5_s()
        => ExecutionTest("14.1-5-s");

    [Fact(DisplayName = "14.1-6-s.js")]
    public Task _14_1_6_s()
        => ExecutionTest("14.1-6-s");

    [Fact(DisplayName = "14.1-7-s.js")]
    public Task _14_1_7_s()
        => ExecutionTest("14.1-7-s");

    [Fact(DisplayName = "14.1-8-s.js")]
    public Task _14_1_8_s()
        => ExecutionTest("14.1-8-s");

    [Fact(DisplayName = "14.1-9-s.js")]
    public Task _14_1_9_s()
        => ExecutionTest("14.1-9-s");

    [Fact(DisplayName = "func-decl-final-runtime.js")]
    public Task func_decl_final_runtime()
        => ExecutionTest("func-decl-final-runtime");

    [Fact(DisplayName = "func-decl-inside-func-decl-runtime.js")]
    public Task func_decl_inside_func_decl_runtime()
        => ExecutionTest("func-decl-inside-func-decl-runtime");

    [Fact(DisplayName = "func-decl-no-semi-runtime.js")]
    public Task func_decl_no_semi_runtime()
        => ExecutionTest("func-decl-no-semi-runtime");

    [Fact(DisplayName = "func-decl-not-first-runtime.js")]
    public Task func_decl_not_first_runtime()
        => ExecutionTest("func-decl-not-first-runtime");

    [Fact(DisplayName = "func-decl-runtime.js")]
    public Task func_decl_runtime()
        => ExecutionTest("func-decl-runtime");

    [Fact(DisplayName = "func-expr-final-runtime.js")]
    public Task func_expr_final_runtime()
        => ExecutionTest("func-expr-final-runtime");

    [Fact(DisplayName = "func-expr-inside-func-decl-runtime.js")]
    public Task func_expr_inside_func_decl_runtime()
        => ExecutionTest("func-expr-inside-func-decl-runtime");

    [Fact(DisplayName = "func-expr-no-semi-runtime.js")]
    public Task func_expr_no_semi_runtime()
        => ExecutionTest("func-expr-no-semi-runtime");

    [Fact(DisplayName = "func-expr-not-first-runtime.js")]
    public Task func_expr_not_first_runtime()
        => ExecutionTest("func-expr-not-first-runtime");

    [Fact(DisplayName = "func-expr-runtime.js")]
    public Task func_expr_runtime()
        => ExecutionTest("func-expr-runtime");

    [Fact(DisplayName = "get-accsr-inside-func-expr-runtime.js")]
    public Task get_accsr_inside_func_expr_runtime()
        => ExecutionTest("get-accsr-inside-func-expr-runtime");

    [Fact(DisplayName = "get-accsr-not-first-runtime.js")]
    public Task get_accsr_not_first_runtime()
        => ExecutionTest("get-accsr-not-first-runtime");

    [Fact(DisplayName = "get-accsr-runtime.js")]
    public Task get_accsr_runtime()
        => ExecutionTest("get-accsr-runtime");

    [Fact(DisplayName = "set-accsr-inside-func-expr-runtime.js")]
    public Task set_accsr_inside_func_expr_runtime()
        => ExecutionTest("set-accsr-inside-func-expr-runtime");

    [Fact(DisplayName = "set-accsr-not-first-runtime.js")]
    public Task set_accsr_not_first_runtime()
        => ExecutionTest("set-accsr-not-first-runtime");

    [Fact(DisplayName = "set-accsr-runtime.js")]
    public Task set_accsr_runtime()
        => ExecutionTest("set-accsr-runtime");
}
