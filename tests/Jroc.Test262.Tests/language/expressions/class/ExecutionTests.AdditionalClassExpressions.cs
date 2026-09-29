using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_;

public partial class ExecutionTests
{
    [Fact(DisplayName = "cpn-class-expr-accessors-computed-property-name-from-async-arrow-function-expression.js")]
    public Task ported_cpn_class_expr_accessors_computed_property_name_from_async_arrow_function_expression() => ExecutionTest("cpn-class-expr-accessors-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-arrow-function-expression.js")]
    public Task ported_cpn_class_expr_computed_property_name_from_arrow_function_expression() => ExecutionTest("cpn-class-expr-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-async-arrow-function-expression.js")]
    public Task ported_cpn_class_expr_computed_property_name_from_async_arrow_function_expression() => ExecutionTest("cpn-class-expr-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-expr-computed-property-name-from-function-expression.js")]
    public Task ported_cpn_class_expr_computed_property_name_from_function_expression() => ExecutionTest("cpn-class-expr-computed-property-name-from-function-expression");

    [Fact(DisplayName = "private-static-setter-multiple-evaluations-of-class-factory.js")]
    public Task ported_private_static_setter_multiple_evaluations_of_class_factory() => ExecutionTest("private-static-setter-multiple-evaluations-of-class-factory");

}
