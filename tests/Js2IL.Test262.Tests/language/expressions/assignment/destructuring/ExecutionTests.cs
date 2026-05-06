using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.assignment.destructuring;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.assignment.destructuring") { }

[Fact(DisplayName = "default-expr-throws-iterator-return-get-throws", Skip = "IteratorClose completion precedence is incorrect.")]
    public Task default_expr_throws_iterator_return_get_throws()
        => ExecutionTest("default-expr-throws-iterator-return-get-throws");

[Fact(DisplayName = "default-expr-throws-iterator-return-is-not-callable")]
    public Task default_expr_throws_iterator_return_is_not_callable()
        => ExecutionTest("default-expr-throws-iterator-return-is-not-callable");

[Fact(DisplayName = "iterator-destructuring-property-reference-target-evaluation-order", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task iterator_destructuring_property_reference_target_evaluation_order()
        => ExecutionTest("iterator-destructuring-property-reference-target-evaluation-order");

[Fact(DisplayName = "keyed-destructuring-property-reference-target-evaluation-order-with-bindings", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task keyed_destructuring_property_reference_target_evaluation_order_with_bindings()
        => ExecutionTest("keyed-destructuring-property-reference-target-evaluation-order-with-bindings");
}
