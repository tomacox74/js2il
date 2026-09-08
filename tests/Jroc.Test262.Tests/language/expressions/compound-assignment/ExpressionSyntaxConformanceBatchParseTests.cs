using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.compound_assignment;

public class ExpressionSyntaxConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchParseTests() : base("language/expressions/compound-assignment", "language.expressions.compound_assignment") { }

    [Fact(DisplayName = "srshift-arguments-strict.js")]
    public Task srshift_arguments_strict()
        => CompilationFailureTest("srshift-arguments-strict");

    [Fact(DisplayName = "srshift-eval-strict.js")]
    public Task srshift_eval_strict()
        => CompilationFailureTest("srshift-eval-strict");

    [Fact(DisplayName = "sub-arguments-strict.js")]
    public Task sub_arguments_strict()
        => CompilationFailureTest("sub-arguments-strict");

    [Fact(DisplayName = "sub-eval-strict.js")]
    public Task sub_eval_strict()
        => CompilationFailureTest("sub-eval-strict");

    [Fact(DisplayName = "subtract-non-simple.js")]
    public Task subtract_non_simple()
        => CompilationFailureTest("subtract-non-simple");

    [Fact(DisplayName = "u-right-shift-non-simple.js")]
    public Task u_right_shift_non_simple()
        => CompilationFailureTest("u-right-shift-non-simple");

    [Fact(DisplayName = "urshift-arguments-strict.js")]
    public Task urshift_arguments_strict()
        => CompilationFailureTest("urshift-arguments-strict");

    [Fact(DisplayName = "urshift-eval-strict.js")]
    public Task urshift_eval_strict()
        => CompilationFailureTest("urshift-eval-strict");

    [Fact(DisplayName = "xor-arguments-strict.js")]
    public Task xor_arguments_strict()
        => CompilationFailureTest("xor-arguments-strict");

    [Fact(DisplayName = "xor-eval-strict.js")]
    public Task xor_eval_strict()
        => CompilationFailureTest("xor-eval-strict");

}
