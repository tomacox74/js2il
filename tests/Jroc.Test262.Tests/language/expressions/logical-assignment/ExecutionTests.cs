using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_assignment;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.logical_assignment") { }

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-short-circuit-and")]
    public Task left_hand_side_private_reference_accessor_property_short_circuit_and()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-short-circuit-and");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-short-circuit-nullish")]
    public Task left_hand_side_private_reference_accessor_property_short_circuit_nullish()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-short-circuit-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-accessor-property-short-circuit-or")]
    public Task left_hand_side_private_reference_accessor_property_short_circuit_or()
        => ExecutionTest("left-hand-side-private-reference-accessor-property-short-circuit-or");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-short-circuit-and")]
    public Task left_hand_side_private_reference_data_property_short_circuit_and()
        => ExecutionTest("left-hand-side-private-reference-data-property-short-circuit-and");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-short-circuit-nullish")]
    public Task left_hand_side_private_reference_data_property_short_circuit_nullish()
        => ExecutionTest("left-hand-side-private-reference-data-property-short-circuit-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-data-property-short-circuit-or")]
    public Task left_hand_side_private_reference_data_property_short_circuit_or()
        => ExecutionTest("left-hand-side-private-reference-data-property-short-circuit-or");

    [Fact(DisplayName = "left-hand-side-private-reference-method-short-circuit-nullish")]
    public Task left_hand_side_private_reference_method_short_circuit_nullish()
        => ExecutionTest("left-hand-side-private-reference-method-short-circuit-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-method-short-circuit-or")]
    public Task left_hand_side_private_reference_method_short_circuit_or()
        => ExecutionTest("left-hand-side-private-reference-method-short-circuit-or");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-short-circuit-and")]
    public Task left_hand_side_private_reference_readonly_accessor_property_short_circuit_and()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-short-circuit-and");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-short-circuit-nullish")]
    public Task left_hand_side_private_reference_readonly_accessor_property_short_circuit_nullish()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-short-circuit-nullish");

    [Fact(DisplayName = "left-hand-side-private-reference-readonly-accessor-property-short-circuit-or")]
    public Task left_hand_side_private_reference_readonly_accessor_property_short_circuit_or()
        => ExecutionTest("left-hand-side-private-reference-readonly-accessor-property-short-circuit-or");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-namedevaluation-arrow-function")]
    public Task lgcl_nullish_assignment_operator_namedevaluation_arrow_function()
        => ExecutionTest("lgcl-nullish-assignment-operator-namedevaluation-arrow-function");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-namedevaluation-class-expression")]
    public Task lgcl_nullish_assignment_operator_namedevaluation_class_expression()
        => ExecutionTest("lgcl-nullish-assignment-operator-namedevaluation-class-expression");

    [Fact(DisplayName = "lgcl-nullish-assignment-operator-namedevaluation-function")]
    public Task lgcl_nullish_assignment_operator_namedevaluation_function()
        => ExecutionTest("lgcl-nullish-assignment-operator-namedevaluation-function");
}
