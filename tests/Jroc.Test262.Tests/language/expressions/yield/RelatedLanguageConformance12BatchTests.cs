using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.yield_;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.yield_") { }

    [Fact(DisplayName = "language/expressions/yield/arguments-object-attributes.js")]
    public Task test_arguments_object_attributes()
        => ExecutionTest("arguments-object-attributes");

    [Fact(DisplayName = "language/expressions/yield/formal-parameters-after-reassignment-non-strict.js")]
    public Task test_formal_parameters_after_reassignment_non_strict()
        => ExecutionTest("formal-parameters-after-reassignment-non-strict");

    [Fact(DisplayName = "language/expressions/yield/formal-parameters-after-reassignment-strict.js")]
    public Task test_formal_parameters_after_reassignment_strict()
        => ExecutionTest("formal-parameters-after-reassignment-strict");

    [Fact(DisplayName = "language/expressions/yield/from-with.js")]
    public Task test_from_with()
        => ExecutionTest("from-with");

    [Fact(DisplayName = "language/expressions/yield/in-iteration-stmt.js")]
    public Task test_in_iteration_stmt()
        => CompilationFailureTest("in-iteration-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/yield/in-rltn-expr.js")]
    public Task test_in_rltn_expr()
        => ExecutionTest("in-rltn-expr");

    [Fact(DisplayName = "language/expressions/yield/invalid-left-hand-side.js")]
    public Task test_invalid_left_hand_side()
        => CompilationFailureTest("invalid-left-hand-side", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/yield/star-in-iteration-stmt.js")]
    public Task test_star_in_iteration_stmt()
        => CompilationFailureTest("star-in-iteration-stmt", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/yield/star-in-rltn-expr.js")]
    public Task test_star_in_rltn_expr()
        => ExecutionTest("star-in-rltn-expr");

    [Fact(DisplayName = "language/expressions/yield/star-rhs-iter-nrml-res-done-no-value.js")]
    public Task test_star_rhs_iter_nrml_res_done_no_value()
        => ExecutionTest("star-rhs-iter-nrml-res-done-no-value");

    [Fact(DisplayName = "language/expressions/yield/within-for.js")]
    public Task test_within_for()
        => ExecutionTest("within-for");
}
