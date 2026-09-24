using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_") { }

    [Fact(DisplayName = "accessor-name-inst-computed-yield-expr")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr", preferOutOfProc: true);

    [Fact(DisplayName = "accessor-name-static-computed-yield-expr")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr", preferOutOfProc: true);

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-arrow-function-expression.js")]
    public Task ported_cpn_class_decl_accessors_computed_property_name_from_arrow_function_expression() => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-function-expression.js")]
    public Task ported_cpn_class_decl_accessors_computed_property_name_from_function_expression() => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-function-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-arrow-function-expression.js")]
    public Task ported_cpn_class_decl_computed_property_name_from_arrow_function_expression() => ExecutionTest("cpn-class-decl-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-function-expression.js")]
    public Task ported_cpn_class_decl_computed_property_name_from_function_expression() => ExecutionTest("cpn-class-decl-computed-property-name-from-function-expression");
}
