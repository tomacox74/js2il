using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.logical_assignment;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.logical_assignment") { }

    [Fact(DisplayName = "lgcl-and-assignment-operator-bigint.js")]
    public Task test_lgcl_and_assignment_operator_bigint() => ExecutionTest("lgcl-and-assignment-operator-bigint");

    [Fact(DisplayName = "lgcl-and-assignment-operator-lhs-before-rhs.js")]
    public Task test_lgcl_and_assignment_operator_lhs_before_rhs() => ExecutionTest("lgcl-and-assignment-operator-lhs-before-rhs");

    [Fact(DisplayName = "lgcl-and-assignment-operator-namedevaluation-arrow-function.js")]
    public Task test_lgcl_and_assignment_operator_namedevaluation_arrow_function() => ExecutionTest("lgcl-and-assignment-operator-namedevaluation-arrow-function");

    [Fact(DisplayName = "lgcl-and-assignment-operator-namedevaluation-class-expression.js")]
    public Task test_lgcl_and_assignment_operator_namedevaluation_class_expression() => ExecutionTest("lgcl-and-assignment-operator-namedevaluation-class-expression");

    [Fact(DisplayName = "lgcl-and-assignment-operator-namedevaluation-function.js")]
    public Task test_lgcl_and_assignment_operator_namedevaluation_function() => ExecutionTest("lgcl-and-assignment-operator-namedevaluation-function");

    [Fact(DisplayName = "lgcl-and-assignment-operator-no-set-put.js")]
    public Task test_lgcl_and_assignment_operator_no_set_put() => ExecutionTest("lgcl-and-assignment-operator-no-set-put");

    [Fact(DisplayName = "lgcl-and-assignment-operator-no-set.js")]
    public Task test_lgcl_and_assignment_operator_no_set() => ExecutionTest("lgcl-and-assignment-operator-no-set");

    [Fact(DisplayName = "lgcl-and-assignment-operator-non-extensible.js")]
    public Task test_lgcl_and_assignment_operator_non_extensible() => ExecutionTest("lgcl-and-assignment-operator-non-extensible");

    [Fact(DisplayName = "lgcl-and-assignment-operator-non-writeable-put.js")]
    public Task test_lgcl_and_assignment_operator_non_writeable_put() => ExecutionTest("lgcl-and-assignment-operator-non-writeable-put");

    [Fact(DisplayName = "lgcl-and-assignment-operator-non-writeable.js")]
    public Task test_lgcl_and_assignment_operator_non_writeable() => ExecutionTest("lgcl-and-assignment-operator-non-writeable");

    [Fact(DisplayName = "lgcl-and-assignment-operator-unresolved-lhs.js")]
    public Task test_lgcl_and_assignment_operator_unresolved_lhs() => ExecutionTest("lgcl-and-assignment-operator-unresolved-lhs");

    [Fact(DisplayName = "lgcl-and-assignment-operator-unresolved-rhs-put.js")]
    public Task test_lgcl_and_assignment_operator_unresolved_rhs_put() => ExecutionTest("lgcl-and-assignment-operator-unresolved-rhs-put");

    [Fact(DisplayName = "lgcl-and-assignment-operator-unresolved-rhs.js")]
    public Task test_lgcl_and_assignment_operator_unresolved_rhs() => ExecutionTest("lgcl-and-assignment-operator-unresolved-rhs");

    [Fact(DisplayName = "lgcl-and-assignment-operator.js")]
    public Task test_lgcl_and_assignment_operator() => ExecutionTest("lgcl-and-assignment-operator");

    [Fact(DisplayName = "lgcl-and-whitespace.js")]
    public Task test_lgcl_and_whitespace() => ExecutionTest("lgcl-and-whitespace");

    [Fact(DisplayName = "lgcl-or-assignment-operator-bigint.js")]
    public Task test_lgcl_or_assignment_operator_bigint() => ExecutionTest("lgcl-or-assignment-operator-bigint");

    [Fact(DisplayName = "lgcl-or-assignment-operator-lhs-before-rhs.js")]
    public Task test_lgcl_or_assignment_operator_lhs_before_rhs() => ExecutionTest("lgcl-or-assignment-operator-lhs-before-rhs");

    [Fact(DisplayName = "lgcl-or-assignment-operator-namedevaluation-arrow-function.js")]
    public Task test_lgcl_or_assignment_operator_namedevaluation_arrow_function() => ExecutionTest("lgcl-or-assignment-operator-namedevaluation-arrow-function");

    [Fact(DisplayName = "lgcl-or-assignment-operator-namedevaluation-class-expression.js")]
    public Task test_lgcl_or_assignment_operator_namedevaluation_class_expression() => ExecutionTest("lgcl-or-assignment-operator-namedevaluation-class-expression");

    [Fact(DisplayName = "lgcl-or-assignment-operator-namedevaluation-function.js")]
    public Task test_lgcl_or_assignment_operator_namedevaluation_function() => ExecutionTest("lgcl-or-assignment-operator-namedevaluation-function");

    [Fact(DisplayName = "lgcl-or-assignment-operator-no-set-put.js")]
    public Task test_lgcl_or_assignment_operator_no_set_put() => ExecutionTest("lgcl-or-assignment-operator-no-set-put");

    [Fact(DisplayName = "lgcl-or-assignment-operator-no-set.js")]
    public Task test_lgcl_or_assignment_operator_no_set() => ExecutionTest("lgcl-or-assignment-operator-no-set");

    [Fact(DisplayName = "lgcl-or-assignment-operator-non-extensible.js")]
    public Task test_lgcl_or_assignment_operator_non_extensible() => ExecutionTest("lgcl-or-assignment-operator-non-extensible");

    [Fact(DisplayName = "lgcl-or-assignment-operator-non-writeable-put.js")]
    public Task test_lgcl_or_assignment_operator_non_writeable_put() => ExecutionTest("lgcl-or-assignment-operator-non-writeable-put");

    [Fact(DisplayName = "lgcl-or-assignment-operator-non-writeable.js")]
    public Task test_lgcl_or_assignment_operator_non_writeable() => ExecutionTest("lgcl-or-assignment-operator-non-writeable");

    [Fact(DisplayName = "lgcl-or-assignment-operator-unresolved-lhs.js")]
    public Task test_lgcl_or_assignment_operator_unresolved_lhs() => ExecutionTest("lgcl-or-assignment-operator-unresolved-lhs");

    [Fact(DisplayName = "lgcl-or-assignment-operator-unresolved-rhs-put.js")]
    public Task test_lgcl_or_assignment_operator_unresolved_rhs_put() => ExecutionTest("lgcl-or-assignment-operator-unresolved-rhs-put");

    [Fact(DisplayName = "lgcl-or-assignment-operator-unresolved-rhs.js")]
    public Task test_lgcl_or_assignment_operator_unresolved_rhs() => ExecutionTest("lgcl-or-assignment-operator-unresolved-rhs");

    [Fact(DisplayName = "lgcl-or-assignment-operator.js")]
    public Task test_lgcl_or_assignment_operator() => ExecutionTest("lgcl-or-assignment-operator");

    [Fact(DisplayName = "lgcl-or-whitespace.js")]
    public Task test_lgcl_or_whitespace() => ExecutionTest("lgcl-or-whitespace");
}
