using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.generators;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.generators") { }

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "generator-created-after-decl-inst")]
    public Task generator_created_after_decl_inst()
        => ExecutionTest("generator-created-after-decl-inst");

    [Fact(DisplayName = "implicit-name")]
    public Task implicit_name()
        => ExecutionTest("implicit-name");

    [Fact(DisplayName = "invoke-as-constructor")]
    public Task invoke_as_constructor()
        => ExecutionTest("invoke-as-constructor");

    [Fact(DisplayName = "length-dflt")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "length-property-descriptor")]
    public Task length_property_descriptor()
        => ExecutionTest("length-property-descriptor");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "no-name")]
    public Task no_name()
        => ExecutionTest("no-name");

    [Fact(DisplayName = "no-yield")]
    public Task no_yield()
        => ExecutionTest("no-yield");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "prototype-relation-to-function")]
    public Task prototype_relation_to_function()
        => ExecutionTest("prototype-relation-to-function");

    [Fact(DisplayName = "prototype-typeof")]
    public Task prototype_typeof()
        => ExecutionTest("prototype-typeof");

    [Fact(DisplayName = "prototype-uniqueness")]
    public Task prototype_uniqueness()
        => ExecutionTest("prototype-uniqueness");

    [Fact(DisplayName = "return")]
    public Task return_()
        => ExecutionTest("return");

    [Fact(DisplayName = "scope-param-elem-var-close")]
    public Task scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open")]
    public Task scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");

    [Fact(DisplayName = "scope-param-rest-elem-var-close")]
    public Task scope_param_rest_elem_var_close()
        => ExecutionTest("scope-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-param-rest-elem-var-open")]
    public Task scope_param_rest_elem_var_open()
        => ExecutionTest("scope-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");

    [Fact(DisplayName = "yield-as-function-expression-binding-identifier")]
    public Task yield_as_function_expression_binding_identifier()
        => ExecutionTest("yield-as-function-expression-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-in-nested-function")]
    public Task yield_as_identifier_in_nested_function()
        => ExecutionTest("yield-as-identifier-in-nested-function");

    [Fact(DisplayName = "yield-as-literal-property-name")]
    public Task yield_as_literal_property_name()
        => ExecutionTest("yield-as-literal-property-name");

    [Fact(DisplayName = "yield-as-property-name")]
    public Task yield_as_property_name()
        => ExecutionTest("yield-as-property-name");

    [Fact(DisplayName = "yield-as-statement")]
    public Task yield_as_statement()
        => ExecutionTest("yield-as-statement");

    [Fact(DisplayName = "yield-as-yield-operand")]
    public Task yield_as_yield_operand()
        => ExecutionTest("yield-as-yield-operand");

    [Fact(DisplayName = "yield-newline")]
    public Task yield_newline()
        => ExecutionTest("yield-newline");

    [Fact(DisplayName = "yield-star-before-newline")]
    public Task yield_star_before_newline()
        => ExecutionTest("yield-star-before-newline");
}
