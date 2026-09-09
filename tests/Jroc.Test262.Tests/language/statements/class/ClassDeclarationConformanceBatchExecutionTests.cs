using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_;

public class ClassDeclarationConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassDeclarationConformanceBatchExecutionTests() : base("language/statements/class", "language.statements.class_") { }

    [Fact(DisplayName = "computed-err-to-prop-key.js")]
    public Task test_accessor_name_inst_computed_err_to_prop_key()
        => ExecutionTest("accessor-name-inst/computed-err-to-prop-key");

    [Fact(DisplayName = "computed-err-to-prop-key.js")]
    public Task test_accessor_name_static_computed_err_to_prop_key()
        => ExecutionTest("accessor-name-static/computed-err-to-prop-key");

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task test_async_gen_method_static_dflt_params_abrupt()
        => ExecutionTest("async-gen-method-static/dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task test_async_gen_method_static_dflt_params_ref_later()
        => ExecutionTest("async-gen-method-static/dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task test_async_gen_method_static_dflt_params_ref_self()
        => ExecutionTest("async-gen-method-static/dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task test_async_gen_method_dflt_params_abrupt()
        => ExecutionTest("async-gen-method/dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task test_async_gen_method_dflt_params_ref_later()
        => ExecutionTest("async-gen-method/dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task test_async_gen_method_dflt_params_ref_self()
        => ExecutionTest("async-gen-method/dflt-params-ref-self");

    [Fact(DisplayName = "class-name-ident-await-escaped.js")]
    public Task test_class_name_ident_await_escaped()
        => ExecutionTest("class-name-ident-await-escaped");

    [Fact(DisplayName = "class-name-ident-await.js")]
    public Task test_class_name_ident_await()
        => ExecutionTest("class-name-ident-await");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-assignment-expression-assignment.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_assignment_expression_assignment()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-assignment-expression-assignment");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-assignment-expression-bitwise-or.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_assignment_expression_bitwise_or()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-assignment-expression-bitwise-or");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-assignment-expression-coalesce.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_assignment_expression_coalesce()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-assignment-expression-coalesce");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-decimal-e-notational-literal.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_decimal_e_notational_literal()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-decimal-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-exponetiation-expression.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_exponetiation_expression()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-exponetiation-expression");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-function-declaration.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_function_declaration()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-integer-e-notational-literal.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_integer_e_notational_literal()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-integer-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-integer-separators.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_integer_separators()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-integer-separators");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-multiplicative-expression-div.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_multiplicative_expression_div()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-multiplicative-expression-div");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-multiplicative-expression-mult.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_multiplicative_expression_mult()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-multiplicative-expression-mult");

    [Fact(DisplayName = "cpn-class-decl-accessors-computed-property-name-from-yield-expression.js")]
    public Task test_cpn_class_decl_accessors_computed_property_name_from_yield_expression()
        => ExecutionTest("cpn-class-decl-accessors-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-assignment-expression-coalesce.js")]
    public Task test_cpn_class_decl_computed_property_name_from_assignment_expression_coalesce()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-assignment-expression-coalesce");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-exponetiation-expression.js")]
    public Task test_cpn_class_decl_computed_property_name_from_exponetiation_expression()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-exponetiation-expression");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-function-declaration.js")]
    public Task test_cpn_class_decl_computed_property_name_from_function_declaration()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-integer-separators.js")]
    public Task test_cpn_class_decl_computed_property_name_from_integer_separators()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-integer-separators");

    [Fact(DisplayName = "cpn-class-decl-computed-property-name-from-yield-expression.js")]
    public Task test_cpn_class_decl_computed_property_name_from_yield_expression()
        => ExecutionTest("cpn-class-decl-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-arrow-function-expression.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-assignment-expression-assignment.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_assignment_expression_assignment()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-assignment-expression-assignment");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-assignment-expression-bitwise-or.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_assignment_expression_bitwise_or()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-assignment-expression-bitwise-or");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-assignment-expression-coalesce.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_assignment_expression_coalesce()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-assignment-expression-coalesce");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-async-arrow-function-expression.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_async_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-decimal-e-notational-literal.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_decimal_e_notational_literal()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-decimal-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-exponetiation-expression.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_exponetiation_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-exponetiation-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-expression-logical-or.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_expression_logical_or()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-function-declaration.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_function_declaration()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-function-expression.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_function_expression()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-integer-e-notational-literal.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_integer_e_notational_literal()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-integer-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-integer-separators.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_integer_separators()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-integer-separators");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-math.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_math()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-math");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-multiplicative-expression-div.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_multiplicative_expression_div()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-multiplicative-expression-div");

    [Fact(DisplayName = "cpn-class-decl-fields-computed-property-name-from-multiplicative-expression-mult.js")]
    public Task test_cpn_class_decl_fields_computed_property_name_from_multiplicative_expression_mult()
        => ExecutionTest("cpn-class-decl-fields-computed-property-name-from-multiplicative-expression-mult");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-additive-expression-add.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_additive_expression_add()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-additive-expression-add");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-additive-expression-subtract.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_additive_expression_subtract()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-additive-expression-subtract");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-arrow-function-expression.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-assignment.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_assignment_expression_assignment()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-assignment");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-bitwise-or.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_assignment_expression_bitwise_or()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-bitwise-or");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-coalesce.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_assignment_expression_coalesce()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-assignment-expression-coalesce");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-async-arrow-function-expression.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_async_arrow_function_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-condition-expression-false.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_condition_expression_false()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-condition-expression-false");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-condition-expression-true.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_condition_expression_true()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-condition-expression-true");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-decimal-e-notational-literal.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_decimal_e_notational_literal()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-decimal-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-decimal-literal.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_decimal_literal()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-decimal-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-exponetiation-expression.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_exponetiation_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-exponetiation-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-expression-coalesce.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_expression_coalesce()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-expression-coalesce");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-expression-logical-and.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_expression_logical_and()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-expression-logical-and");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-expression-logical-or.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_expression_logical_or()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-expression-logical-or");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-function-declaration.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_function_declaration()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-function-declaration");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-function-expression.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_function_expression()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-function-expression");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-identifier.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_identifier()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-integer-e-notational-literal.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_integer_e_notational_literal()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-integer-e-notational-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-integer-separators.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_integer_separators()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-integer-separators");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-math.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_math()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-math");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-multiplicative-expression-div.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_multiplicative_expression_div()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-multiplicative-expression-div");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-multiplicative-expression-mult.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_multiplicative_expression_mult()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-multiplicative-expression-mult");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-null.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_null()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-numeric-literal.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_numeric_literal()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-class-decl-fields-methods-computed-property-name-from-string-literal.js")]
    public Task test_cpn_class_decl_fields_methods_computed_property_name_from_string_literal()
        => ExecutionTest("cpn-class-decl-fields-methods-computed-property-name-from-string-literal");

    [Fact(DisplayName = "class-method-returns-promise.js")]
    public Task test_definition_class_method_returns_promise()
        => ExecutionTest("definition/class-method-returns-promise");

    [Fact(DisplayName = "fn-name-gen-method.js")]
    public Task test_definition_fn_name_gen_method()
        => ExecutionTest("definition/fn-name-gen-method");

    [Fact(DisplayName = "fn-name-method.js")]
    public Task test_definition_fn_name_method()
        => ExecutionTest("definition/fn-name-method");

    [Fact(DisplayName = "fn-name-static-precedence.js")]
    public Task test_definition_fn_name_static_precedence()
        => ExecutionTest("definition/fn-name-static-precedence");

    [Fact(DisplayName = "getters-restricted-ids.js")]
    public Task test_definition_getters_restricted_ids()
        => ExecutionTest("definition/getters-restricted-ids");

    [Fact(DisplayName = "invalid-extends.js")]
    public Task test_definition_invalid_extends()
        => ExecutionTest("definition/invalid-extends");

    [Fact(DisplayName = "methods-gen-no-yield.js")]
    public Task test_definition_methods_gen_no_yield()
        => ExecutionTest("definition/methods-gen-no-yield");

    [Fact(DisplayName = "methods-gen-return.js")]
    public Task test_definition_methods_gen_return()
        => ExecutionTest("definition/methods-gen-return");

    [Fact(DisplayName = "methods-gen-yield-as-expression-with-rhs.js")]
    public Task test_definition_methods_gen_yield_as_expression_with_rhs()
        => ExecutionTest("definition/methods-gen-yield-as-expression-with-rhs");

    [Fact(DisplayName = "methods-gen-yield-as-expression-without-rhs.js")]
    public Task test_definition_methods_gen_yield_as_expression_without_rhs()
        => ExecutionTest("definition/methods-gen-yield-as-expression-without-rhs");

    [Fact(DisplayName = "methods-gen-yield-as-generator-method-binding-identifier.js")]
    public Task test_definition_methods_gen_yield_as_generator_method_binding_identifier()
        => ExecutionTest("definition/methods-gen-yield-as-generator-method-binding-identifier");

    [Fact(DisplayName = "methods-gen-yield-as-literal-property-name.js")]
    public Task test_definition_methods_gen_yield_as_literal_property_name()
        => ExecutionTest("definition/methods-gen-yield-as-literal-property-name");

    [Fact(DisplayName = "methods-gen-yield-as-property-name.js")]
    public Task test_definition_methods_gen_yield_as_property_name()
        => ExecutionTest("definition/methods-gen-yield-as-property-name");

    [Fact(DisplayName = "methods-gen-yield-as-statement.js")]
    public Task test_definition_methods_gen_yield_as_statement()
        => ExecutionTest("definition/methods-gen-yield-as-statement");

    [Fact(DisplayName = "methods-gen-yield-as-yield-operand.js")]
    public Task test_definition_methods_gen_yield_as_yield_operand()
        => ExecutionTest("definition/methods-gen-yield-as-yield-operand");

    [Fact(DisplayName = "methods-gen-yield-newline.js")]
    public Task test_definition_methods_gen_yield_newline()
        => ExecutionTest("definition/methods-gen-yield-newline");

    [Fact(DisplayName = "methods-gen-yield-star-before-newline.js")]
    public Task test_definition_methods_gen_yield_star_before_newline()
        => ExecutionTest("definition/methods-gen-yield-star-before-newline");

    [Fact(DisplayName = "methods-named-eval-arguments.js")]
    public Task test_definition_methods_named_eval_arguments()
        => ExecutionTest("definition/methods-named-eval-arguments");

    [Fact(DisplayName = "methods-restricted-properties.js")]
    public Task test_definition_methods_restricted_properties()
        => ExecutionTest("definition/methods-restricted-properties");

    [Fact(DisplayName = "setters-prop-desc.js")]
    public Task test_definition_setters_prop_desc()
        => ExecutionTest("definition/setters-prop-desc");

    [Fact(DisplayName = "setters-restricted-ids.js")]
    public Task test_definition_setters_restricted_ids()
        => ExecutionTest("definition/setters-restricted-ids");

    [Fact(DisplayName = "side-effects-in-extends.js")]
    public Task test_definition_side_effects_in_extends()
        => ExecutionTest("definition/side-effects-in-extends");

    [Fact(DisplayName = "side-effects-in-property-define.js")]
    public Task test_definition_side_effects_in_property_define()
        => ExecutionTest("definition/side-effects-in-property-define");

    [Fact(DisplayName = "this-check-ordering.js")]
    public Task test_definition_this_check_ordering()
        => ExecutionTest("definition/this-check-ordering");

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_async_gen_meth_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/async-gen-meth-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_async_gen_meth_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/async-gen-meth-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_async_gen_meth_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/async-gen-meth-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_async_gen_meth_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/async-gen-meth-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_async_gen_meth_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/async-gen-meth-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_async_gen_meth_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/async-gen-meth-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_async_gen_meth_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/async-gen-meth-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_async_gen_meth_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/async-gen-meth-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-init-null.js")]
    public Task test_dstr_async_gen_meth_dflt_obj_init_null()
        => ExecutionTest("dstr/async-gen-meth-dflt-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-init-undefined.js")]
    public Task test_dstr_async_gen_meth_dflt_obj_init_undefined()
        => ExecutionTest("dstr/async-gen-meth-dflt-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_async_gen_meth_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/async-gen-meth-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_async_gen_meth_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/async-gen-meth-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_async_gen_meth_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/async-gen-meth-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-gen-meth-obj-init-null.js")]
    public Task test_dstr_async_gen_meth_obj_init_null()
        => ExecutionTest("dstr/async-gen-meth-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-obj-init-undefined.js")]
    public Task test_dstr_async_gen_meth_obj_init_undefined()
        => ExecutionTest("dstr/async-gen-meth-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_async_gen_meth_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/async-gen-meth-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_async_gen_meth_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/async-gen-meth-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_async_gen_meth_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/async-gen-meth-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-gen-meth-static-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_async_gen_meth_static_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/async-gen-meth-static-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-static-ary-init-iter-get-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_init_iter_get_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_async_gen_meth_static_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/async-gen-meth-static-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-init-iter-get-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-init-null.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_init_null()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-init-undefined.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_init_undefined()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-list-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-static-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_async_gen_meth_static_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/async-gen-meth-static-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-gen-meth-static-obj-init-null.js")]
    public Task test_dstr_async_gen_meth_static_obj_init_null()
        => ExecutionTest("dstr/async-gen-meth-static-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-static-obj-init-undefined.js")]
    public Task test_dstr_async_gen_meth_static_obj_init_undefined()
        => ExecutionTest("dstr/async-gen-meth-static-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-list-err.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_list_err()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-static-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_async_gen_meth_static_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/async-gen-meth-static-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-ary-init-iter-close.js")]
    public Task test_dstr_gen_meth_ary_init_iter_close()
        => ExecutionTest("dstr/gen-meth-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-ary-init-iter-no-close.js")]
    public Task test_dstr_gen_meth_ary_init_iter_no_close()
        => ExecutionTest("dstr/gen-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-ary-name-iter-val.js")]
    public Task test_dstr_gen_meth_ary_name_iter_val()
        => ExecutionTest("dstr/gen-meth-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elision.js")]
    public Task test_dstr_gen_meth_ary_ptrn_elision()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-empty.js")]
    public Task test_dstr_gen_meth_ary_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_id()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_gen_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-close.js")]
    public Task test_dstr_gen_meth_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/gen-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_gen_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/gen-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-dflt-ary-name-iter-val.js")]
    public Task test_dstr_gen_meth_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/gen-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_gen_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_gen_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/gen-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "gen-meth-obj-ptrn-empty.js")]
    public Task test_dstr_gen_meth_obj_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_gen_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_gen_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_gen_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id.js")]
    public Task test_dstr_gen_meth_obj_ptrn_prop_id()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-getter.js")]
    public Task test_dstr_gen_meth_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_gen_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_gen_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/gen-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "gen-meth-static-ary-init-iter-close.js")]
    public Task test_dstr_gen_meth_static_ary_init_iter_close()
        => ExecutionTest("dstr/gen-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-static-ary-init-iter-no-close.js")]
    public Task test_dstr_gen_meth_static_ary_init_iter_no_close()
        => ExecutionTest("dstr/gen-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-static-ary-name-iter-val.js")]
    public Task test_dstr_gen_meth_static_ary_name_iter_val()
        => ExecutionTest("dstr/gen-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elision.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_elision()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-empty.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_gen_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-init-iter-close.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-name-iter-val.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_gen_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_gen_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/gen-meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-empty.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_empty()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-rest-getter.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_gen_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/gen-meth-static-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-ary-init-iter-close.js")]
    public Task test_dstr_meth_ary_init_iter_close()
        => ExecutionTest("dstr/meth-ary-init-iter-close");

    [Fact(DisplayName = "meth-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_meth_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/meth-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-ary-init-iter-get-err.js")]
    public Task test_dstr_meth_ary_init_iter_get_err()
        => ExecutionTest("dstr/meth-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-ary-init-iter-no-close.js")]
    public Task test_dstr_meth_ary_init_iter_no_close()
        => ExecutionTest("dstr/meth-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-ary-name-iter-val.js")]
    public Task test_dstr_meth_ary_name_iter_val()
        => ExecutionTest("dstr/meth-ary-name-iter-val");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_meth_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_meth_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/meth-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_meth_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/meth-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-elision.js")]
    public Task test_dstr_meth_ary_ptrn_elision()
        => ExecutionTest("dstr/meth-ary-ptrn-elision");

    [Fact(DisplayName = "meth-ary-ptrn-empty.js")]
    public Task test_dstr_meth_ary_ptrn_empty()
        => ExecutionTest("dstr/meth-ary-ptrn-empty");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id.js")]
    public Task test_dstr_meth_ary_ptrn_rest_id()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-close.js")]
    public Task test_dstr_meth_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_meth_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/meth-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-get-err.js")]
    public Task test_dstr_meth_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/meth-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-dflt-ary-name-iter-val.js")]
    public Task test_dstr_meth_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-exhausted");
}
