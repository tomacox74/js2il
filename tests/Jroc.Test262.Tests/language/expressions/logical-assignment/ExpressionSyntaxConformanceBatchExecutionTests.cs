using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.logical_assignment;

public class ExpressionSyntaxConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ExpressionSyntaxConformanceBatchExecutionTests() : base("language/expressions/logical-assignment", "language.expressions.logical_assignment") { }

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-and.js")]
    public Task left_hand_side_private_reference_accessor_property_and()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-and");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-nullish.js")]
    public Task left_hand_side_private_reference_accessor_property_nullish()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-or.js")]
    public Task left_hand_side_private_reference_accessor_property_or()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-or");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-and.js")]
    public Task left_hand_side_private_reference_data_property_and()
        => ExecutionTest("left-hand-side-private-reference-data-property-and");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-nullish.js")]
    public Task left_hand_side_private_reference_data_property_nullish()
        => ExecutionTest("left-hand-side-private-reference-data-property-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-or.js")]
    public Task left_hand_side_private_reference_data_property_or()
        => ExecutionTest("left-hand-side-private-reference-data-property-or");

    [Fact(DisplayName = "left-hand-side-private-reference-method-and.js")]
    public Task left_hand_side_private_reference_method_and()
        => ExecutionTest("left-hand-side-private-reference-method-and");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-and.js")]
    public Task left_hand_side_private_reference_readonly_accessor_property_and()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-and");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-nullish.js")]
    public Task left_hand_side_private_reference_readonly_accessor_property_nullish()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-or.js")]
    public Task left_hand_side_private_reference_readonly_accessor_property_or()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-or");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-bigint.js")]
    public Task lgcl_nullish_assignment_operator_bigint()
        => ExecutionTest("lgcl-nullish-assignment-operator-bigint");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-lhs-before-rhs.js")]
    public Task lgcl_nullish_assignment_operator_lhs_before_rhs()
        => ExecutionTest("lgcl-nullish-assignment-operator-lhs-before-rhs");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-no-set-put.js")]
    public Task lgcl_nullish_assignment_operator_no_set_put()
        => ExecutionTest("lgcl-nullish-assignment-operator-no-set-put");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-no-set.js")]
    public Task lgcl_nullish_assignment_operator_no_set()
        => ExecutionTest("lgcl-nullish-assignment-operator-no-set");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-non-extensible.js")]
    public Task lgcl_nullish_assignment_operator_non_extensible()
        => ExecutionTest("lgcl-nullish-assignment-operator-non-extensible");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-non-writeable-put.js")]
    public Task lgcl_nullish_assignment_operator_non_writeable_put()
        => ExecutionTest("lgcl-nullish-assignment-operator-non-writeable-put");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-non-writeable.js")]
    public Task lgcl_nullish_assignment_operator_non_writeable()
        => ExecutionTest("lgcl-nullish-assignment-operator-non-writeable");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-unresolved-lhs.js")]
    public Task lgcl_nullish_assignment_operator_unresolved_lhs()
        => ExecutionTest("lgcl-nullish-assignment-operator-unresolved-lhs");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-unresolved-rhs-put.js")]
    public Task lgcl_nullish_assignment_operator_unresolved_rhs_put()
        => ExecutionTest("lgcl-nullish-assignment-operator-unresolved-rhs-put");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-unresolved-rhs.js")]
    public Task lgcl_nullish_assignment_operator_unresolved_rhs()
        => ExecutionTest("lgcl-nullish-assignment-operator-unresolved-rhs");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator.js")]
    public Task lgcl_nullish_assignment_operator()
        => ExecutionTest("lgcl-nullish-assignment-operator");

    [Fact(DisplayName = "lgcl-nullish-whitespace.js")]
    public Task lgcl_nullish_whitespace()
        => ExecutionTest("lgcl-nullish-whitespace");

}
