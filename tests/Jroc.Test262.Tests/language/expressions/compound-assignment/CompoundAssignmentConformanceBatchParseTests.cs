using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.compound_assignment;

public class CompoundAssignmentConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public CompoundAssignmentConformanceBatchParseTests() : base("language/expressions/compound-assignment", "language.expressions.compound_assignment") { }

    [Fact(DisplayName = "11.13.2-6-1gs.js")]
    public Task _11_13_2_6_1gs()
        => CompilationFailureTest("11.13.2-6-1gs");

    [Fact(DisplayName = "add-arguments-strict.js")]
    public Task add_arguments_strict()
        => CompilationFailureTest("add-arguments-strict");

    [Fact(DisplayName = "add-eval-strict.js")]
    public Task add_eval_strict()
        => CompilationFailureTest("add-eval-strict");

    [Fact(DisplayName = "add-non-simple.js")]
    public Task add_non_simple()
        => CompilationFailureTest("add-non-simple");

    [Fact(DisplayName = "and-arguments-strict.js")]
    public Task and_arguments_strict()
        => CompilationFailureTest("and-arguments-strict");

    [Fact(DisplayName = "and-eval-strict.js")]
    public Task and_eval_strict()
        => CompilationFailureTest("and-eval-strict");

    [Fact(DisplayName = "btws-and-non-simple.js")]
    public Task btws_and_non_simple()
        => CompilationFailureTest("btws-and-non-simple");

    [Fact(DisplayName = "btws-or-non-simple.js")]
    public Task btws_or_non_simple()
        => CompilationFailureTest("btws-or-non-simple");

    [Fact(DisplayName = "btws-xor-non-simple.js")]
    public Task btws_xor_non_simple()
        => CompilationFailureTest("btws-xor-non-simple");

    [Fact(DisplayName = "div-arguments-strict.js")]
    public Task div_arguments_strict()
        => CompilationFailureTest("div-arguments-strict");

    [Fact(DisplayName = "div-eval-strict.js")]
    public Task div_eval_strict()
        => CompilationFailureTest("div-eval-strict");

    [Fact(DisplayName = "div-non-simple.js")]
    public Task div_non_simple()
        => CompilationFailureTest("div-non-simple");

    [Fact(DisplayName = "left-shift-non-simple.js")]
    public Task left_shift_non_simple()
        => CompilationFailureTest("left-shift-non-simple");

    [Fact(DisplayName = "lshift-arguments-strict.js")]
    public Task lshift_arguments_strict()
        => CompilationFailureTest("lshift-arguments-strict");

    [Fact(DisplayName = "lshift-eval-strict.js")]
    public Task lshift_eval_strict()
        => CompilationFailureTest("lshift-eval-strict");

    [Fact(DisplayName = "mod-arguments-strict.js")]
    public Task mod_arguments_strict()
        => CompilationFailureTest("mod-arguments-strict");

    [Fact(DisplayName = "mod-div-non-simple.js")]
    public Task mod_div_non_simple()
        => CompilationFailureTest("mod-div-non-simple");

    [Fact(DisplayName = "mod-eval-strict.js")]
    public Task mod_eval_strict()
        => CompilationFailureTest("mod-eval-strict");

    [Fact(DisplayName = "mult-arguments-strict.js")]
    public Task mult_arguments_strict()
        => CompilationFailureTest("mult-arguments-strict");

    [Fact(DisplayName = "mult-eval-strict.js")]
    public Task mult_eval_strict()
        => CompilationFailureTest("mult-eval-strict");

    [Fact(DisplayName = "mult-non-simple.js")]
    public Task mult_non_simple()
        => CompilationFailureTest("mult-non-simple");

    [Fact(DisplayName = "or-arguments-strict.js")]
    public Task or_arguments_strict()
        => CompilationFailureTest("or-arguments-strict");

    [Fact(DisplayName = "or-eval-strict.js")]
    public Task or_eval_strict()
        => CompilationFailureTest("or-eval-strict");

    [Fact(DisplayName = "right-shift-non-simple.js")]
    public Task right_shift_non_simple()
        => CompilationFailureTest("right-shift-non-simple");

}
