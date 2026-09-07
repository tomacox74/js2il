using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.object_;

public class LanguageExpressionsObjectConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LanguageExpressionsObjectConformanceBatchExecutionTests() : base("language.expressions.object") { }

    [Fact(DisplayName = "accessor-name-computed-err-evaluation.js")]
    public Task accessor_name_computed_err_evaluation() => ExecutionTest("accessor-name-computed-err-evaluation");

    [Fact(DisplayName = "accessor-name-computed-err-to-prop-key.js")]
    public Task accessor_name_computed_err_to_prop_key() => ExecutionTest("accessor-name-computed-err-to-prop-key");

    [Fact(DisplayName = "accessor-name-computed-err-unresolvable.js")]
    public Task accessor_name_computed_err_unresolvable() => ExecutionTest("accessor-name-computed-err-unresolvable");

    [Fact(DisplayName = "accessor-name-computed-in.js")]
    public Task accessor_name_computed_in() => ExecutionTest("accessor-name-computed-in");

    [Fact(DisplayName = "accessor-name-computed-yield-expr.js")]
    public Task accessor_name_computed_yield_expr() => ExecutionTest("accessor-name-computed-yield-expr");

    [Fact(DisplayName = "accessor-name-computed-yield-id.js")]
    public Task accessor_name_computed_yield_id() => ExecutionTest("accessor-name-computed-yield-id");

    [Fact(DisplayName = "accessor-name-computed.js")]
    public Task accessor_name_computed() => ExecutionTest("accessor-name-computed");

    [Fact(DisplayName = "accessor-name-literal-numeric-binary.js")]
    public Task accessor_name_literal_numeric_binary() => ExecutionTest("accessor-name-literal-numeric-binary");

    [Fact(DisplayName = "accessor-name-literal-numeric-exponent.js")]
    public Task accessor_name_literal_numeric_exponent() => ExecutionTest("accessor-name-literal-numeric-exponent");

    [Fact(DisplayName = "accessor-name-literal-numeric-hex.js")]
    public Task accessor_name_literal_numeric_hex() => ExecutionTest("accessor-name-literal-numeric-hex");

    [Fact(DisplayName = "accessor-name-literal-numeric-leading-decimal.js")]
    public Task accessor_name_literal_numeric_leading_decimal() => ExecutionTest("accessor-name-literal-numeric-leading-decimal");

    [Fact(DisplayName = "accessor-name-literal-numeric-non-canonical.js")]
    public Task accessor_name_literal_numeric_non_canonical() => ExecutionTest("accessor-name-literal-numeric-non-canonical");

    [Fact(DisplayName = "accessor-name-literal-numeric-octal.js")]
    public Task accessor_name_literal_numeric_octal() => ExecutionTest("accessor-name-literal-numeric-octal");

    [Fact(DisplayName = "accessor-name-literal-numeric-zero.js")]
    public Task accessor_name_literal_numeric_zero() => ExecutionTest("accessor-name-literal-numeric-zero");

    [Fact(DisplayName = "accessor-name-literal-string-char-escape.js")]
    public Task accessor_name_literal_string_char_escape() => ExecutionTest("accessor-name-literal-string-char-escape");

    [Fact(DisplayName = "accessor-name-literal-string-default-escaped-ext.js")]
    public Task accessor_name_literal_string_default_escaped_ext() => ExecutionTest("accessor-name-literal-string-default-escaped-ext");

    [Fact(DisplayName = "accessor-name-literal-string-default-escaped.js")]
    public Task accessor_name_literal_string_default_escaped() => ExecutionTest("accessor-name-literal-string-default-escaped");

    [Fact(DisplayName = "accessor-name-literal-string-default.js")]
    public Task accessor_name_literal_string_default() => ExecutionTest("accessor-name-literal-string-default");

    [Fact(DisplayName = "accessor-name-literal-string-double-quote.js")]
    public Task accessor_name_literal_string_double_quote() => ExecutionTest("accessor-name-literal-string-double-quote");

    [Fact(DisplayName = "accessor-name-literal-string-empty.js")]
    public Task accessor_name_literal_string_empty() => ExecutionTest("accessor-name-literal-string-empty");

    [Fact(DisplayName = "accessor-name-literal-string-hex-escape.js")]
    public Task accessor_name_literal_string_hex_escape() => ExecutionTest("accessor-name-literal-string-hex-escape");

    [Fact(DisplayName = "accessor-name-literal-string-line-continuation.js")]
    public Task accessor_name_literal_string_line_continuation() => ExecutionTest("accessor-name-literal-string-line-continuation");

    [Fact(DisplayName = "accessor-name-literal-string-single-quote.js")]
    public Task accessor_name_literal_string_single_quote() => ExecutionTest("accessor-name-literal-string-single-quote");

    [Fact(DisplayName = "accessor-name-literal-string-unicode-escape.js")]
    public Task accessor_name_literal_string_unicode_escape() => ExecutionTest("accessor-name-literal-string-unicode-escape");

    [Fact(DisplayName = "computed-property-evaluation-order.js")]
    public Task computed_property_evaluation_order() => ExecutionTest("computed-property-evaluation-order");

    [Fact(DisplayName = "concise-generator.js")]
    public Task concise_generator() => ExecutionTest("concise-generator");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-break-escaped.js")]
    public Task covered_ident_name_prop_name_literal_break_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-break-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-case-escaped.js")]
    public Task covered_ident_name_prop_name_literal_case_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-case-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-catch-escaped.js")]
    public Task covered_ident_name_prop_name_literal_catch_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-catch-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-class-escaped.js")]
    public Task covered_ident_name_prop_name_literal_class_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-class-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-const-escaped.js")]
    public Task covered_ident_name_prop_name_literal_const_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-const-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-continue-escaped.js")]
    public Task covered_ident_name_prop_name_literal_continue_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-continue-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-debugger-escaped.js")]
    public Task covered_ident_name_prop_name_literal_debugger_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-debugger-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-default-escaped-ext.js")]
    public Task covered_ident_name_prop_name_literal_default_escaped_ext() => ExecutionTest("covered-ident-name-prop-name-literal-default-escaped-ext");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-default-escaped.js")]
    public Task covered_ident_name_prop_name_literal_default_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-default-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-default.js")]
    public Task covered_ident_name_prop_name_literal_default() => ExecutionTest("covered-ident-name-prop-name-literal-default");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-delete-escaped.js")]
    public Task covered_ident_name_prop_name_literal_delete_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-delete-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-do-escaped.js")]
    public Task covered_ident_name_prop_name_literal_do_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-do-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-else-escaped.js")]
    public Task covered_ident_name_prop_name_literal_else_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-else-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-enum-escaped.js")]
    public Task covered_ident_name_prop_name_literal_enum_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-enum-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-export-escaped.js")]
    public Task covered_ident_name_prop_name_literal_export_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-export-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-extends-escaped-ext.js")]
    public Task covered_ident_name_prop_name_literal_extends_escaped_ext() => ExecutionTest("covered-ident-name-prop-name-literal-extends-escaped-ext");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-extends-escaped.js")]
    public Task covered_ident_name_prop_name_literal_extends_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-extends-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-extends.js")]
    public Task covered_ident_name_prop_name_literal_extends() => ExecutionTest("covered-ident-name-prop-name-literal-extends");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-finally-escaped.js")]
    public Task covered_ident_name_prop_name_literal_finally_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-finally-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-for-escaped.js")]
    public Task covered_ident_name_prop_name_literal_for_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-for-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-function-escaped.js")]
    public Task covered_ident_name_prop_name_literal_function_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-function-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-if-escaped.js")]
    public Task covered_ident_name_prop_name_literal_if_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-if-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-implements-escaped.js")]
    public Task covered_ident_name_prop_name_literal_implements_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-implements-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-import-escaped.js")]
    public Task covered_ident_name_prop_name_literal_import_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-import-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-in-escaped.js")]
    public Task covered_ident_name_prop_name_literal_in_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-in-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-instanceof-escaped.js")]
    public Task covered_ident_name_prop_name_literal_instanceof_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-instanceof-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-interface-escaped.js")]
    public Task covered_ident_name_prop_name_literal_interface_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-interface-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-let-escaped.js")]
    public Task covered_ident_name_prop_name_literal_let_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-let-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-new-escaped.js")]
    public Task covered_ident_name_prop_name_literal_new_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-new-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-package-escaped.js")]
    public Task covered_ident_name_prop_name_literal_package_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-package-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-private-escaped.js")]
    public Task covered_ident_name_prop_name_literal_private_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-private-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-protected-escaped.js")]
    public Task covered_ident_name_prop_name_literal_protected_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-protected-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-public-escaped.js")]
    public Task covered_ident_name_prop_name_literal_public_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-public-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-return-escaped.js")]
    public Task covered_ident_name_prop_name_literal_return_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-return-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-static-escaped.js")]
    public Task covered_ident_name_prop_name_literal_static_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-static-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-super-escaped.js")]
    public Task covered_ident_name_prop_name_literal_super_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-super-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-switch-escaped.js")]
    public Task covered_ident_name_prop_name_literal_switch_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-switch-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-this-escaped.js")]
    public Task covered_ident_name_prop_name_literal_this_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-this-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-throw-escaped.js")]
    public Task covered_ident_name_prop_name_literal_throw_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-throw-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-try-escaped.js")]
    public Task covered_ident_name_prop_name_literal_try_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-try-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-typeof-escaped.js")]
    public Task covered_ident_name_prop_name_literal_typeof_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-typeof-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-var-escaped.js")]
    public Task covered_ident_name_prop_name_literal_var_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-var-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-void-escaped.js")]
    public Task covered_ident_name_prop_name_literal_void_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-void-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-while-escaped.js")]
    public Task covered_ident_name_prop_name_literal_while_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-while-escaped");

    [Fact(DisplayName = "covered-ident-name-prop-name-literal-with-escaped.js")]
    public Task covered_ident_name_prop_name_literal_with_escaped() => ExecutionTest("covered-ident-name-prop-name-literal-with-escaped");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-additive-expression-add.js")]
    public Task cpn_obj_lit_computed_property_name_from_additive_expression_add() => ExecutionTest("cpn-obj-lit-computed-property-name-from-additive-expression-add");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-additive-expression-subtract.js")]
    public Task cpn_obj_lit_computed_property_name_from_additive_expression_subtract() => ExecutionTest("cpn-obj-lit-computed-property-name-from-additive-expression-subtract");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-arrow-function-expression.js")]
    public Task cpn_obj_lit_computed_property_name_from_arrow_function_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-arrow-function-expression");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-assignment-expression-assignment.js")]
    public Task cpn_obj_lit_computed_property_name_from_assignment_expression_assignment() => ExecutionTest("cpn-obj-lit-computed-property-name-from-assignment-expression-assignment");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-assignment-expression-bitwise-or.js")]
    public Task cpn_obj_lit_computed_property_name_from_assignment_expression_bitwise_or() => ExecutionTest("cpn-obj-lit-computed-property-name-from-assignment-expression-bitwise-or");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-assignment-expression-coalesce.js")]
    public Task cpn_obj_lit_computed_property_name_from_assignment_expression_coalesce() => ExecutionTest("cpn-obj-lit-computed-property-name-from-assignment-expression-coalesce");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-async-arrow-function-expression.js")]
    public Task cpn_obj_lit_computed_property_name_from_async_arrow_function_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-async-arrow-function-expression");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-condition-expression-false.js")]
    public Task cpn_obj_lit_computed_property_name_from_condition_expression_false() => ExecutionTest("cpn-obj-lit-computed-property-name-from-condition-expression-false");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-condition-expression-true.js")]
    public Task cpn_obj_lit_computed_property_name_from_condition_expression_true() => ExecutionTest("cpn-obj-lit-computed-property-name-from-condition-expression-true");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-decimal-e-notational-literal.js")]
    public Task cpn_obj_lit_computed_property_name_from_decimal_e_notational_literal() => ExecutionTest("cpn-obj-lit-computed-property-name-from-decimal-e-notational-literal");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-decimal-literal.js")]
    public Task cpn_obj_lit_computed_property_name_from_decimal_literal() => ExecutionTest("cpn-obj-lit-computed-property-name-from-decimal-literal");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-exponetiation-expression.js")]
    public Task cpn_obj_lit_computed_property_name_from_exponetiation_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-exponetiation-expression");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-expression-coalesce.js")]
    public Task cpn_obj_lit_computed_property_name_from_expression_coalesce() => ExecutionTest("cpn-obj-lit-computed-property-name-from-expression-coalesce");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-expression-logical-and.js")]
    public Task cpn_obj_lit_computed_property_name_from_expression_logical_and() => ExecutionTest("cpn-obj-lit-computed-property-name-from-expression-logical-and");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-expression-logical-or.js")]
    public Task cpn_obj_lit_computed_property_name_from_expression_logical_or() => ExecutionTest("cpn-obj-lit-computed-property-name-from-expression-logical-or");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-function-declaration.js")]
    public Task cpn_obj_lit_computed_property_name_from_function_declaration() => ExecutionTest("cpn-obj-lit-computed-property-name-from-function-declaration");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-function-expression.js")]
    public Task cpn_obj_lit_computed_property_name_from_function_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-function-expression");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-identifier.js")]
    public Task cpn_obj_lit_computed_property_name_from_identifier() => ExecutionTest("cpn-obj-lit-computed-property-name-from-identifier");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-integer-e-notational-literal.js")]
    public Task cpn_obj_lit_computed_property_name_from_integer_e_notational_literal() => ExecutionTest("cpn-obj-lit-computed-property-name-from-integer-e-notational-literal");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-integer-separators.js")]
    public Task cpn_obj_lit_computed_property_name_from_integer_separators() => ExecutionTest("cpn-obj-lit-computed-property-name-from-integer-separators");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-math.js")]
    public Task cpn_obj_lit_computed_property_name_from_math() => ExecutionTest("cpn-obj-lit-computed-property-name-from-math");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-multiplicative-expression-div.js")]
    public Task cpn_obj_lit_computed_property_name_from_multiplicative_expression_div() => ExecutionTest("cpn-obj-lit-computed-property-name-from-multiplicative-expression-div");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-multiplicative-expression-mult.js")]
    public Task cpn_obj_lit_computed_property_name_from_multiplicative_expression_mult() => ExecutionTest("cpn-obj-lit-computed-property-name-from-multiplicative-expression-mult");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-null.js")]
    public Task cpn_obj_lit_computed_property_name_from_null() => ExecutionTest("cpn-obj-lit-computed-property-name-from-null");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-numeric-literal.js")]
    public Task cpn_obj_lit_computed_property_name_from_numeric_literal() => ExecutionTest("cpn-obj-lit-computed-property-name-from-numeric-literal");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-string-literal.js")]
    public Task cpn_obj_lit_computed_property_name_from_string_literal() => ExecutionTest("cpn-obj-lit-computed-property-name-from-string-literal");

    [Fact(DisplayName = "cpn-obj-lit-computed-property-name-from-yield-expression.js")]
    public Task cpn_obj_lit_computed_property_name_from_yield_expression() => ExecutionTest("cpn-obj-lit-computed-property-name-from-yield-expression");

    [Fact(DisplayName = "fn-name-accessor-get.js")]
    public Task fn_name_accessor_get() => ExecutionTest("fn-name-accessor-get");

    [Fact(DisplayName = "fn-name-accessor-set.js")]
    public Task fn_name_accessor_set() => ExecutionTest("fn-name-accessor-set");

    [Fact(DisplayName = "fn-name-arrow.js")]
    public Task fn_name_arrow() => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-fn.js")]
    public Task fn_name_fn() => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen.js")]
    public Task fn_name_gen() => ExecutionTest("fn-name-gen");

    [Fact(DisplayName = "getter-prop-desc.js")]
    public Task getter_prop_desc() => ExecutionTest("getter-prop-desc");

    [Fact(DisplayName = "getter-super-prop.js")]
    public Task getter_super_prop() => ExecutionTest("getter-super-prop");

    [Fact(DisplayName = "ident-name-method-def-break-escaped.js")]
    public Task ident_name_method_def_break_escaped() => ExecutionTest("ident-name-method-def-break-escaped");

    [Fact(DisplayName = "ident-name-method-def-case-escaped.js")]
    public Task ident_name_method_def_case_escaped() => ExecutionTest("ident-name-method-def-case-escaped");

    [Fact(DisplayName = "ident-name-method-def-catch-escaped.js")]
    public Task ident_name_method_def_catch_escaped() => ExecutionTest("ident-name-method-def-catch-escaped");

    [Fact(DisplayName = "ident-name-method-def-class-escaped.js")]
    public Task ident_name_method_def_class_escaped() => ExecutionTest("ident-name-method-def-class-escaped");

    [Fact(DisplayName = "ident-name-method-def-const-escaped.js")]
    public Task ident_name_method_def_const_escaped() => ExecutionTest("ident-name-method-def-const-escaped");

    [Fact(DisplayName = "ident-name-method-def-continue-escaped.js")]
    public Task ident_name_method_def_continue_escaped() => ExecutionTest("ident-name-method-def-continue-escaped");

    [Fact(DisplayName = "ident-name-method-def-debugger-escaped.js")]
    public Task ident_name_method_def_debugger_escaped() => ExecutionTest("ident-name-method-def-debugger-escaped");

    [Fact(DisplayName = "ident-name-method-def-default-escaped-ext.js")]
    public Task ident_name_method_def_default_escaped_ext() => ExecutionTest("ident-name-method-def-default-escaped-ext");

    [Fact(DisplayName = "ident-name-method-def-default-escaped.js")]
    public Task ident_name_method_def_default_escaped() => ExecutionTest("ident-name-method-def-default-escaped");

    [Fact(DisplayName = "ident-name-method-def-default.js")]
    public Task ident_name_method_def_default() => ExecutionTest("ident-name-method-def-default");

    [Fact(DisplayName = "ident-name-method-def-delete-escaped.js")]
    public Task ident_name_method_def_delete_escaped() => ExecutionTest("ident-name-method-def-delete-escaped");

    [Fact(DisplayName = "ident-name-method-def-do-escaped.js")]
    public Task ident_name_method_def_do_escaped() => ExecutionTest("ident-name-method-def-do-escaped");

    [Fact(DisplayName = "ident-name-method-def-else-escaped.js")]
    public Task ident_name_method_def_else_escaped() => ExecutionTest("ident-name-method-def-else-escaped");

    [Fact(DisplayName = "ident-name-method-def-enum-escaped.js")]
    public Task ident_name_method_def_enum_escaped() => ExecutionTest("ident-name-method-def-enum-escaped");

    [Fact(DisplayName = "ident-name-method-def-export-escaped.js")]
    public Task ident_name_method_def_export_escaped() => ExecutionTest("ident-name-method-def-export-escaped");

    [Fact(DisplayName = "ident-name-method-def-extends-escaped-ext.js")]
    public Task ident_name_method_def_extends_escaped_ext() => ExecutionTest("ident-name-method-def-extends-escaped-ext");

    [Fact(DisplayName = "ident-name-method-def-extends-escaped.js")]
    public Task ident_name_method_def_extends_escaped() => ExecutionTest("ident-name-method-def-extends-escaped");

    [Fact(DisplayName = "ident-name-method-def-extends.js")]
    public Task ident_name_method_def_extends() => ExecutionTest("ident-name-method-def-extends");

    [Fact(DisplayName = "ident-name-method-def-finally-escaped.js")]
    public Task ident_name_method_def_finally_escaped() => ExecutionTest("ident-name-method-def-finally-escaped");

    [Fact(DisplayName = "ident-name-method-def-for-escaped.js")]
    public Task ident_name_method_def_for_escaped() => ExecutionTest("ident-name-method-def-for-escaped");

    [Fact(DisplayName = "ident-name-method-def-function-escaped.js")]
    public Task ident_name_method_def_function_escaped() => ExecutionTest("ident-name-method-def-function-escaped");

    [Fact(DisplayName = "ident-name-method-def-if-escaped.js")]
    public Task ident_name_method_def_if_escaped() => ExecutionTest("ident-name-method-def-if-escaped");

}
