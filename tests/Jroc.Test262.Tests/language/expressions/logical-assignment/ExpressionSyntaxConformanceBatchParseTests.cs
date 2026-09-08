using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.logical_assignment;

public class ExpressionSyntaxConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchParseTests() : base("language/expressions/logical-assignment", "language.expressions.logical_assignment") { }

    [Fact(DisplayName = "lgcl-and-arguments-strict.js")]
    public Task lgcl_and_arguments_strict()
        => CompilationFailureTest("lgcl-and-arguments-strict");

    [Fact(DisplayName = "lgcl-and-assignment-operator-non-simple-lhs.js")]
    public Task lgcl_and_assignment_operator_non_simple_lhs()
        => CompilationFailureTest("lgcl-and-assignment-operator-non-simple-lhs");

    [Fact(DisplayName = "lgcl-and-eval-strict.js")]
    public Task lgcl_and_eval_strict()
        => CompilationFailureTest("lgcl-and-eval-strict");

    [Fact(DisplayName = "lgcl-and-non-simple.js")]
    public Task lgcl_and_non_simple()
        => CompilationFailureTest("lgcl-and-non-simple");

    [Fact(DisplayName = "lgcl-nullish-arguments-strict.js")]
    public Task lgcl_nullish_arguments_strict()
        => CompilationFailureTest("lgcl-nullish-arguments-strict");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-non-simple-lhs.js")]
    public Task lgcl_nullish_assignment_operator_non_simple_lhs()
        => CompilationFailureTest("lgcl-nullish-assignment-operator-non-simple-lhs");

    [Fact(DisplayName = "lgcl-nullish-eval-strict.js")]
    public Task lgcl_nullish_eval_strict()
        => CompilationFailureTest("lgcl-nullish-eval-strict");

    [Fact(DisplayName = "lgcl-nullish-non-simple.js")]
    public Task lgcl_nullish_non_simple()
        => CompilationFailureTest("lgcl-nullish-non-simple");

    [Fact(DisplayName = "lgcl-or-arguments-strict.js")]
    public Task lgcl_or_arguments_strict()
        => CompilationFailureTest("lgcl-or-arguments-strict");

    [Fact(DisplayName = "lgcl-or-assignment-operator-non-simple-lhs.js")]
    public Task lgcl_or_assignment_operator_non_simple_lhs()
        => CompilationFailureTest("lgcl-or-assignment-operator-non-simple-lhs");

    [Fact(DisplayName = "lgcl-or-eval-strict.js")]
    public Task lgcl_or_eval_strict()
        => CompilationFailureTest("lgcl-or-eval-strict");

    [Fact(DisplayName = "lgcl-or-non-simple.js")]
    public Task lgcl_or_non_simple()
        => CompilationFailureTest("lgcl-or-non-simple");

}
