namespace Jroc.Test262.Tests.language.statements.test_class.elements;

public sealed class ClassElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassElementsExecutionTests() : base("language.statements.test_class.elements") { }

    [Fact(DisplayName = "after-same-line-gen-computed-names.js")]
    public Task after_same_line_gen_computed_names()
        => ExecutionTest("after-same-line-gen-computed-names");

    [Fact(DisplayName = "after-same-line-gen-literal-names-asi.js")]
    public Task after_same_line_gen_literal_names_asi()
        => ExecutionTest("after-same-line-gen-literal-names-asi");

    [Fact(DisplayName = "after-same-line-gen-literal-names.js")]
    public Task after_same_line_gen_literal_names()
        => ExecutionTest("after-same-line-gen-literal-names");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task after_same_line_gen_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-by-classname.js")]
    public Task after_same_line_gen_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task after_same_line_gen_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "after-same-line-gen-static-private-fields.js")]
    public Task after_same_line_gen_static_private_fields()
        => ExecutionTest("after-same-line-gen-static-private-fields");

    [Fact(DisplayName = "after-same-line-gen-static-private-methods-with-fields.js")]
    public Task after_same_line_gen_static_private_methods_with_fields()
        => ExecutionTest("after-same-line-gen-static-private-methods-with-fields");

    [Fact(DisplayName = "after-same-line-method-computed-names.js")]
    public Task after_same_line_method_computed_names()
        => ExecutionTest("after-same-line-method-computed-names");

    [Fact(DisplayName = "after-same-line-method-literal-names-asi.js")]
    public Task after_same_line_method_literal_names_asi()
        => ExecutionTest("after-same-line-method-literal-names-asi");

    [Fact(DisplayName = "after-same-line-method-literal-names.js")]
    public Task after_same_line_method_literal_names()
        => ExecutionTest("after-same-line-method-literal-names");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task after_same_line_method_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-by-classname.js")]
    public Task after_same_line_method_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task after_same_line_method_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "after-same-line-method-static-private-fields.js")]
    public Task after_same_line_method_static_private_fields()
        => ExecutionTest("after-same-line-method-static-private-fields");

    [Fact(DisplayName = "after-same-line-method-static-private-methods-with-fields.js")]
    public Task after_same_line_method_static_private_methods_with_fields()
        => ExecutionTest("after-same-line-method-static-private-methods-with-fields");

    [Fact(DisplayName = "after-same-line-static-gen-computed-symbol-names.js")]
    public Task after_same_line_static_gen_computed_symbol_names()
        => ExecutionTest("after-same-line-static-gen-computed-symbol-names");

    [Fact(DisplayName = "after-same-line-static-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task after_same_line_static_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("after-same-line-static-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "after-same-line-static-gen-literal-names-asi.js")]
    public Task after_same_line_static_gen_literal_names_asi()
        => ExecutionTest("after-same-line-static-gen-literal-names-asi");

    [Fact(DisplayName = "after-same-line-static-gen-literal-names.js")]
    public Task after_same_line_static_gen_literal_names()
        => ExecutionTest("after-same-line-static-gen-literal-names");

    [Fact(DisplayName = "after-same-line-static-gen-private-field-usage.js")]
    public Task after_same_line_static_gen_private_field_usage()
        => ExecutionTest("after-same-line-static-gen-private-field-usage");

    [Fact(DisplayName = "after-same-line-static-gen-private-names.js")]
    public Task after_same_line_static_gen_private_names()
        => ExecutionTest("after-same-line-static-gen-private-names");

    [Fact(DisplayName = "after-same-line-static-gen-rs-field-identifier-initializer.js")]
    public Task after_same_line_static_gen_rs_field_identifier_initializer()
        => ExecutionTest("after-same-line-static-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-gen-rs-field-identifier.js")]
    public Task after_same_line_static_gen_rs_field_identifier()
        => ExecutionTest("after-same-line-static-gen-rs-field-identifier");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-getter-alt.js")]
    public Task after_same_line_static_gen_rs_private_getter_alt()
        => ExecutionTest("after-same-line-static-gen-rs-private-getter-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-getter.js")]
    public Task after_same_line_static_gen_rs_private_getter()
        => ExecutionTest("after-same-line-static-gen-rs-private-getter");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-method-alt.js")]
    public Task after_same_line_static_gen_rs_private_method_alt()
        => ExecutionTest("after-same-line-static-gen-rs-private-method-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-method.js")]
    public Task after_same_line_static_gen_rs_private_method()
        => ExecutionTest("after-same-line-static-gen-rs-private-method");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-setter-alt.js")]
    public Task after_same_line_static_gen_rs_private_setter_alt()
        => ExecutionTest("after-same-line-static-gen-rs-private-setter-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-private-setter.js")]
    public Task after_same_line_static_gen_rs_private_setter()
        => ExecutionTest("after-same-line-static-gen-rs-private-setter");

    [Fact(DisplayName = "after-same-line-static-gen-rs-privatename-identifier-alt.js")]
    public Task after_same_line_static_gen_rs_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task after_same_line_static_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("after-same-line-static-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-privatename-identifier-initializer.js")]
    public Task after_same_line_static_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("after-same-line-static-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-gen-rs-privatename-identifier.js")]
    public Task after_same_line_static_gen_rs_privatename_identifier()
        => ExecutionTest("after-same-line-static-gen-rs-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task after_same_line_static_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task after_same_line_static_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("after-same-line-static-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task after_same_line_static_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-method-privatename-identifier.js")]
    public Task after_same_line_static_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("after-same-line-static-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task after_same_line_static_gen_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-by-classname.js")]
    public Task after_same_line_static_gen_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task after_same_line_static_gen_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "after-same-line-static-gen-static-private-fields.js")]
    public Task after_same_line_static_gen_static_private_fields()
        => ExecutionTest("after-same-line-static-gen-static-private-fields");

    [Fact(DisplayName = "after-same-line-static-gen-static-private-methods-with-fields.js")]
    public Task after_same_line_static_gen_static_private_methods_with_fields()
        => ExecutionTest("after-same-line-static-gen-static-private-methods-with-fields");

    [Fact(DisplayName = "after-same-line-static-gen-static-private-methods.js")]
    public Task after_same_line_static_gen_static_private_methods()
        => ExecutionTest("after-same-line-static-gen-static-private-methods");

    [Fact(DisplayName = "after-same-line-static-method-computed-names.js")]
    public Task after_same_line_static_method_computed_names()
        => ExecutionTest("after-same-line-static-method-computed-names");

    [Fact(DisplayName = "after-same-line-static-method-computed-symbol-names.js")]
    public Task after_same_line_static_method_computed_symbol_names()
        => ExecutionTest("after-same-line-static-method-computed-symbol-names");

    [Fact(DisplayName = "after-same-line-static-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task after_same_line_static_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("after-same-line-static-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "after-same-line-static-method-literal-names-asi.js")]
    public Task after_same_line_static_method_literal_names_asi()
        => ExecutionTest("after-same-line-static-method-literal-names-asi");

    [Fact(DisplayName = "after-same-line-static-method-literal-names.js")]
    public Task after_same_line_static_method_literal_names()
        => ExecutionTest("after-same-line-static-method-literal-names");

    [Fact(DisplayName = "after-same-line-static-method-private-field-usage.js")]
    public Task after_same_line_static_method_private_field_usage()
        => ExecutionTest("after-same-line-static-method-private-field-usage");

    [Fact(DisplayName = "after-same-line-static-method-rs-field-identifier-initializer.js")]
    public Task after_same_line_static_method_rs_field_identifier_initializer()
        => ExecutionTest("after-same-line-static-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-method-rs-field-identifier.js")]
    public Task after_same_line_static_method_rs_field_identifier()
        => ExecutionTest("after-same-line-static-method-rs-field-identifier");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-getter-alt.js")]
    public Task after_same_line_static_method_rs_private_getter_alt()
        => ExecutionTest("after-same-line-static-method-rs-private-getter-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-getter.js")]
    public Task after_same_line_static_method_rs_private_getter()
        => ExecutionTest("after-same-line-static-method-rs-private-getter");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-method-alt.js")]
    public Task after_same_line_static_method_rs_private_method_alt()
        => ExecutionTest("after-same-line-static-method-rs-private-method-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-method.js")]
    public Task after_same_line_static_method_rs_private_method()
        => ExecutionTest("after-same-line-static-method-rs-private-method");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-setter-alt.js")]
    public Task after_same_line_static_method_rs_private_setter_alt()
        => ExecutionTest("after-same-line-static-method-rs-private-setter-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-private-setter.js")]
    public Task after_same_line_static_method_rs_private_setter()
        => ExecutionTest("after-same-line-static-method-rs-private-setter");

    [Fact(DisplayName = "after-same-line-static-method-rs-privatename-identifier-alt.js")]
    public Task after_same_line_static_method_rs_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-privatename-identifier-initializer-alt.js")]
    public Task after_same_line_static_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("after-same-line-static-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-privatename-identifier-initializer.js")]
    public Task after_same_line_static_method_rs_privatename_identifier_initializer()
        => ExecutionTest("after-same-line-static-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-method-rs-privatename-identifier.js")]
    public Task after_same_line_static_method_rs_privatename_identifier()
        => ExecutionTest("after-same-line-static-method-rs-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task after_same_line_static_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-generator-method-privatename-identifier.js")]
    public Task after_same_line_static_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("after-same-line-static-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-method-privatename-identifier-alt.js")]
    public Task after_same_line_static_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("after-same-line-static-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-method-privatename-identifier.js")]
    public Task after_same_line_static_method_rs_static_method_privatename_identifier()
        => ExecutionTest("after-same-line-static-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task after_same_line_static_method_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-by-classname.js")]
    public Task after_same_line_static_method_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task after_same_line_static_method_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "after-same-line-static-method-static-private-fields.js")]
    public Task after_same_line_static_method_static_private_fields()
        => ExecutionTest("after-same-line-static-method-static-private-fields");

    [Fact(DisplayName = "after-same-line-static-method-static-private-methods-with-fields.js")]
    public Task after_same_line_static_method_static_private_methods_with_fields()
        => ExecutionTest("after-same-line-static-method-static-private-methods-with-fields");

    [Fact(DisplayName = "after-same-line-static-method-static-private-methods.js")]
    public Task after_same_line_static_method_static_private_methods()
        => ExecutionTest("after-same-line-static-method-static-private-methods");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("arrow-body-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("arrow-body-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("arrow-body-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("arrow-body-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("arrow-body-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("arrow-body-derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("arrow-body-derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("arrow-body-derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("arrow-body-derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("arrow-body-derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task arrow_body_direct_eval_err_contains_arguments()
        => ExecutionTest("arrow-body-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "arrow-body-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_direct_eval_err_contains_newtarget()
        => ExecutionTest("arrow-body-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "arrow-body-indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_indirect_eval_err_contains_newtarget()
        => ExecutionTest("arrow-body-indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("arrow-body-private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("arrow-body-private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("arrow-body-private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("arrow-body-private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTest("arrow-body-private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-private-derived-cls-indirect-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_indirect_eval_contains_superproperty_1()
        => ExecutionTest("arrow-body-private-derived-cls-indirect-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-indirect-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_indirect_eval_contains_superproperty_2()
        => ExecutionTest("arrow-body-private-derived-cls-indirect-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-indirect-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_indirect_eval_err_contains_supercall_1()
        => ExecutionTest("arrow-body-private-derived-cls-indirect-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-indirect-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_indirect_eval_err_contains_supercall_2()
        => ExecutionTest("arrow-body-private-derived-cls-indirect-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-indirect-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_indirect_eval_err_contains_supercall()
        => ExecutionTest("arrow-body-private-derived-cls-indirect-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_direct_eval_err_contains_arguments()
        => ExecutionTest("arrow-body-private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "arrow-body-private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_direct_eval_err_contains_newtarget()
        => ExecutionTest("arrow-body-private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "arrow-body-private-indirect-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_indirect_eval_err_contains_newtarget()
        => ExecutionTest("arrow-body-private-indirect-eval-err-contains-newtarget");

    [Fact(DisplayName = "computed-name-toprimitive.js")]
    public Task computed_name_toprimitive()
        => ExecutionTest("computed-name-toprimitive");

    [Fact(DisplayName = "computed-property-abrupt-completition.js")]
    public Task computed_property_abrupt_completition()
        => ExecutionTest("computed-property-abrupt-completition");

    [Fact(DisplayName = "derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTest("derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTest("derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTest("derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTest("derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "field-declaration.js")]
    public Task field_declaration()
        => ExecutionTest("field-declaration");

    [Fact(DisplayName = "field-definition-accessor-no-line-terminator.js")]
    public Task field_definition_accessor_no_line_terminator()
        => ExecutionTest("field-definition-accessor-no-line-terminator");

    [Fact(DisplayName = "fields-computed-name-static-computed-var-propname-constructor.js")]
    public Task fields_computed_name_static_computed_var_propname_constructor()
        => ExecutionTest("fields-computed-name-static-computed-var-propname-constructor");

    [Fact(DisplayName = "fields-computed-name-static-propname-constructor.js")]
    public Task fields_computed_name_static_propname_constructor()
        => ExecutionTest("fields-computed-name-static-propname-constructor");

    [Fact(DisplayName = "fields-hash-constructor-is-a-valid-name.js")]
    public Task fields_hash_constructor_is_a_valid_name()
        => ExecutionTest("fields-hash-constructor-is-a-valid-name");

    [Fact(DisplayName = "get-access-of-missing-private-getter.js")]
    public Task get_access_of_missing_private_getter()
        => ExecutionTest("get-access-of-missing-private-getter");

    [Fact(DisplayName = "get-access-of-missing-private-static-getter.js")]
    public Task get_access_of_missing_private_static_getter()
        => ExecutionTest("get-access-of-missing-private-static-getter");

    [Fact(DisplayName = "get-access-of-missing-shadowed-private-getter.js")]
    public Task get_access_of_missing_shadowed_private_getter()
        => ExecutionTest("get-access-of-missing-shadowed-private-getter");

    [Fact(DisplayName = "grammar-private-field-optional-chaining.js")]
    public Task grammar_private_field_optional_chaining()
        => ExecutionTest("grammar-private-field-optional-chaining");

    [Fact(DisplayName = "init-value-defined-after-class.js")]
    public Task init_value_defined_after_class()
        => ExecutionTest("init-value-defined-after-class");

    [Fact(DisplayName = "init-value-incremental.js")]
    public Task init_value_incremental()
        => ExecutionTest("init-value-incremental");

    [Fact(DisplayName = "intercalated-static-non-static-computed-fields.js")]
    public Task intercalated_static_non_static_computed_fields()
        => ExecutionTest("intercalated-static-non-static-computed-fields");

    [Fact(DisplayName = "multiple-definitions-computed-names.js")]
    public Task multiple_definitions_computed_names()
        => ExecutionTest("multiple-definitions-computed-names");

    [Fact(DisplayName = "multiple-definitions-computed-symbol-names.js")]
    public Task multiple_definitions_computed_symbol_names()
        => ExecutionTest("multiple-definitions-computed-symbol-names");

    [Fact(DisplayName = "multiple-definitions-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task multiple_definitions_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("multiple-definitions-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "multiple-definitions-literal-names-asi.js")]
    public Task multiple_definitions_literal_names_asi()
        => ExecutionTest("multiple-definitions-literal-names-asi");

    [Fact(DisplayName = "multiple-definitions-literal-names.js")]
    public Task multiple_definitions_literal_names()
        => ExecutionTest("multiple-definitions-literal-names");

    [Fact(DisplayName = "multiple-definitions-private-field-usage.js")]
    public Task multiple_definitions_private_field_usage()
        => ExecutionTest("multiple-definitions-private-field-usage");

    [Fact(DisplayName = "multiple-definitions-rs-field-identifier-initializer.js")]
    public Task multiple_definitions_rs_field_identifier_initializer()
        => ExecutionTest("multiple-definitions-rs-field-identifier-initializer");

    [Fact(DisplayName = "multiple-definitions-rs-field-identifier.js")]
    public Task multiple_definitions_rs_field_identifier()
        => ExecutionTest("multiple-definitions-rs-field-identifier");

    [Fact(DisplayName = "multiple-definitions-rs-private-getter-alt.js")]
    public Task multiple_definitions_rs_private_getter_alt()
        => ExecutionTest("multiple-definitions-rs-private-getter-alt");

    [Fact(DisplayName = "multiple-definitions-rs-private-getter.js")]
    public Task multiple_definitions_rs_private_getter()
        => ExecutionTest("multiple-definitions-rs-private-getter");

    [Fact(DisplayName = "multiple-definitions-rs-private-method-alt.js")]
    public Task multiple_definitions_rs_private_method_alt()
        => ExecutionTest("multiple-definitions-rs-private-method-alt");

    [Fact(DisplayName = "multiple-definitions-rs-private-method.js")]
    public Task multiple_definitions_rs_private_method()
        => ExecutionTest("multiple-definitions-rs-private-method");

    [Fact(DisplayName = "multiple-definitions-rs-private-setter-alt.js")]
    public Task multiple_definitions_rs_private_setter_alt()
        => ExecutionTest("multiple-definitions-rs-private-setter-alt");

    [Fact(DisplayName = "multiple-definitions-rs-private-setter.js")]
    public Task multiple_definitions_rs_private_setter()
        => ExecutionTest("multiple-definitions-rs-private-setter");

    [Fact(DisplayName = "multiple-definitions-rs-privatename-identifier-alt.js")]
    public Task multiple_definitions_rs_privatename_identifier_alt()
        => ExecutionTest("multiple-definitions-rs-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-definitions-rs-privatename-identifier-initializer-alt.js")]
    public Task multiple_definitions_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("multiple-definitions-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "multiple-definitions-rs-privatename-identifier-initializer.js")]
    public Task multiple_definitions_rs_privatename_identifier_initializer()
        => ExecutionTest("multiple-definitions-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "multiple-definitions-rs-privatename-identifier.js")]
    public Task multiple_definitions_rs_privatename_identifier()
        => ExecutionTest("multiple-definitions-rs-privatename-identifier");

    [Fact(DisplayName = "multiple-definitions-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task multiple_definitions_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("multiple-definitions-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-definitions-rs-static-generator-method-privatename-identifier.js")]
    public Task multiple_definitions_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("multiple-definitions-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "multiple-definitions-rs-static-method-privatename-identifier-alt.js")]
    public Task multiple_definitions_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("multiple-definitions-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-definitions-rs-static-method-privatename-identifier.js")]
    public Task multiple_definitions_rs_static_method_privatename_identifier()
        => ExecutionTest("multiple-definitions-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task multiple_definitions_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-by-classname.js")]
    public Task multiple_definitions_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task multiple_definitions_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "multiple-definitions-static-private-fields.js")]
    public Task multiple_definitions_static_private_fields()
        => ExecutionTest("multiple-definitions-static-private-fields");

    [Fact(DisplayName = "multiple-definitions-static-private-methods-with-fields.js")]
    public Task multiple_definitions_static_private_methods_with_fields()
        => ExecutionTest("multiple-definitions-static-private-methods-with-fields");

    [Fact(DisplayName = "multiple-definitions-static-private-methods.js")]
    public Task multiple_definitions_static_private_methods()
        => ExecutionTest("multiple-definitions-static-private-methods");

    [Fact(DisplayName = "multiple-stacked-definitions-computed-names.js")]
    public Task multiple_stacked_definitions_computed_names()
        => ExecutionTest("multiple-stacked-definitions-computed-names");

    [Fact(DisplayName = "multiple-stacked-definitions-computed-symbol-names.js")]
    public Task multiple_stacked_definitions_computed_symbol_names()
        => ExecutionTest("multiple-stacked-definitions-computed-symbol-names");

    [Fact(DisplayName = "multiple-stacked-definitions-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task multiple_stacked_definitions_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("multiple-stacked-definitions-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "multiple-stacked-definitions-literal-names-asi.js")]
    public Task multiple_stacked_definitions_literal_names_asi()
        => ExecutionTest("multiple-stacked-definitions-literal-names-asi");

    [Fact(DisplayName = "multiple-stacked-definitions-literal-names.js")]
    public Task multiple_stacked_definitions_literal_names()
        => ExecutionTest("multiple-stacked-definitions-literal-names");

    [Fact(DisplayName = "multiple-stacked-definitions-private-field-usage.js")]
    public Task multiple_stacked_definitions_private_field_usage()
        => ExecutionTest("multiple-stacked-definitions-private-field-usage");

    [Fact(DisplayName = "multiple-stacked-definitions-private-method-getter-usage.js")]
    public Task multiple_stacked_definitions_private_method_getter_usage()
        => ExecutionTest("multiple-stacked-definitions-private-method-getter-usage");

    [Fact(DisplayName = "multiple-stacked-definitions-private-method-usage.js")]
    public Task multiple_stacked_definitions_private_method_usage()
        => ExecutionTest("multiple-stacked-definitions-private-method-usage");

    [Fact(DisplayName = "multiple-stacked-definitions-private-names.js")]
    public Task multiple_stacked_definitions_private_names()
        => ExecutionTest("multiple-stacked-definitions-private-names");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-field-identifier-initializer.js")]
    public Task multiple_stacked_definitions_rs_field_identifier_initializer()
        => ExecutionTest("multiple-stacked-definitions-rs-field-identifier-initializer");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-field-identifier.js")]
    public Task multiple_stacked_definitions_rs_field_identifier()
        => ExecutionTest("multiple-stacked-definitions-rs-field-identifier");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-getter-alt.js")]
    public Task multiple_stacked_definitions_rs_private_getter_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-private-getter-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-getter.js")]
    public Task multiple_stacked_definitions_rs_private_getter()
        => ExecutionTest("multiple-stacked-definitions-rs-private-getter");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-method-alt.js")]
    public Task multiple_stacked_definitions_rs_private_method_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-private-method-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-method.js")]
    public Task multiple_stacked_definitions_rs_private_method()
        => ExecutionTest("multiple-stacked-definitions-rs-private-method");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-setter-alt.js")]
    public Task multiple_stacked_definitions_rs_private_setter_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-private-setter-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-private-setter.js")]
    public Task multiple_stacked_definitions_rs_private_setter()
        => ExecutionTest("multiple-stacked-definitions-rs-private-setter");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-privatename-identifier-alt.js")]
    public Task multiple_stacked_definitions_rs_privatename_identifier_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-privatename-identifier-initializer-alt.js")]
    public Task multiple_stacked_definitions_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-privatename-identifier-initializer.js")]
    public Task multiple_stacked_definitions_rs_privatename_identifier_initializer()
        => ExecutionTest("multiple-stacked-definitions-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-privatename-identifier.js")]
    public Task multiple_stacked_definitions_rs_privatename_identifier()
        => ExecutionTest("multiple-stacked-definitions-rs-privatename-identifier");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task multiple_stacked_definitions_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-generator-method-privatename-identifier.js")]
    public Task multiple_stacked_definitions_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("multiple-stacked-definitions-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-method-privatename-identifier-alt.js")]
    public Task multiple_stacked_definitions_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("multiple-stacked-definitions-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-method-privatename-identifier.js")]
    public Task multiple_stacked_definitions_rs_static_method_privatename_identifier()
        => ExecutionTest("multiple-stacked-definitions-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task multiple_stacked_definitions_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-by-classname.js")]
    public Task multiple_stacked_definitions_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task multiple_stacked_definitions_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "multiple-stacked-definitions-static-private-fields.js")]
    public Task multiple_stacked_definitions_static_private_fields()
        => ExecutionTest("multiple-stacked-definitions-static-private-fields");

    [Fact(DisplayName = "multiple-stacked-definitions-static-private-methods-with-fields.js")]
    public Task multiple_stacked_definitions_static_private_methods_with_fields()
        => ExecutionTest("multiple-stacked-definitions-static-private-methods-with-fields");

    [Fact(DisplayName = "multiple-stacked-definitions-static-private-methods.js")]
    public Task multiple_stacked_definitions_static_private_methods()
        => ExecutionTest("multiple-stacked-definitions-static-private-methods");

    [Fact(DisplayName = "new-no-sc-line-method-computed-names.js")]
    public Task new_no_sc_line_method_computed_names()
        => ExecutionTest("new-no-sc-line-method-computed-names");

    [Fact(DisplayName = "new-no-sc-line-method-literal-names-asi.js")]
    public Task new_no_sc_line_method_literal_names_asi()
        => ExecutionTest("new-no-sc-line-method-literal-names-asi");

    [Fact(DisplayName = "new-no-sc-line-method-literal-names.js")]
    public Task new_no_sc_line_method_literal_names()
        => ExecutionTest("new-no-sc-line-method-literal-names");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task new_no_sc_line_method_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-by-classname.js")]
    public Task new_no_sc_line_method_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task new_no_sc_line_method_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "new-no-sc-line-method-static-private-fields.js")]
    public Task new_no_sc_line_method_static_private_fields()
        => ExecutionTest("new-no-sc-line-method-static-private-fields");

    [Fact(DisplayName = "new-no-sc-line-method-static-private-methods-with-fields.js")]
    public Task new_no_sc_line_method_static_private_methods_with_fields()
        => ExecutionTest("new-no-sc-line-method-static-private-methods-with-fields");

    [Fact(DisplayName = "new-sc-line-gen-computed-names.js")]
    public Task new_sc_line_gen_computed_names()
        => ExecutionTest("new-sc-line-gen-computed-names");

    [Fact(DisplayName = "new-sc-line-gen-literal-names-asi.js")]
    public Task new_sc_line_gen_literal_names_asi()
        => ExecutionTest("new-sc-line-gen-literal-names-asi");

    [Fact(DisplayName = "new-sc-line-gen-literal-names.js")]
    public Task new_sc_line_gen_literal_names()
        => ExecutionTest("new-sc-line-gen-literal-names");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task new_sc_line_gen_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-by-classname.js")]
    public Task new_sc_line_gen_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task new_sc_line_gen_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "new-sc-line-gen-static-private-fields.js")]
    public Task new_sc_line_gen_static_private_fields()
        => ExecutionTest("new-sc-line-gen-static-private-fields");

    [Fact(DisplayName = "new-sc-line-gen-static-private-methods-with-fields.js")]
    public Task new_sc_line_gen_static_private_methods_with_fields()
        => ExecutionTest("new-sc-line-gen-static-private-methods-with-fields");

    [Fact(DisplayName = "new-sc-line-method-computed-names.js")]
    public Task new_sc_line_method_computed_names()
        => ExecutionTest("new-sc-line-method-computed-names");

    [Fact(DisplayName = "new-sc-line-method-literal-names-asi.js")]
    public Task new_sc_line_method_literal_names_asi()
        => ExecutionTest("new-sc-line-method-literal-names-asi");

    [Fact(DisplayName = "new-sc-line-method-literal-names.js")]
    public Task new_sc_line_method_literal_names()
        => ExecutionTest("new-sc-line-method-literal-names");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task new_sc_line_method_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-by-classname.js")]
    public Task new_sc_line_method_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task new_sc_line_method_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "new-sc-line-method-static-private-fields.js")]
    public Task new_sc_line_method_static_private_fields()
        => ExecutionTest("new-sc-line-method-static-private-fields");

    [Fact(DisplayName = "new-sc-line-method-static-private-methods-with-fields.js")]
    public Task new_sc_line_method_static_private_methods_with_fields()
        => ExecutionTest("new-sc-line-method-static-private-methods-with-fields");

    [Fact(DisplayName = "private-field-access-on-inner-arrow-function.js")]
    public Task private_field_access_on_inner_arrow_function()
        => ExecutionTest("private-field-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-field-access-on-inner-function.js")]
    public Task private_field_access_on_inner_function()
        => ExecutionTest("private-field-access-on-inner-function");

    [Fact(DisplayName = "private-field-after-optional-chain.js")]
    public Task private_field_after_optional_chain()
        => ExecutionTest("private-field-after-optional-chain");

    [Fact(DisplayName = "private-field-on-nested-class.js")]
    public Task private_field_on_nested_class()
        => ExecutionTest("private-field-on-nested-class");

    [Fact(DisplayName = "private-getter-access-on-inner-arrow-function.js")]
    public Task private_getter_access_on_inner_arrow_function()
        => ExecutionTest("private-getter-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-getter-access-on-inner-function.js")]
    public Task private_getter_access_on_inner_function()
        => ExecutionTest("private-getter-access-on-inner-function");

    [Fact(DisplayName = "private-getter-brand-check.js")]
    public Task private_getter_brand_check()
        => ExecutionTest("private-getter-brand-check");

    [Fact(DisplayName = "private-getter-on-nested-class.js")]
    public Task private_getter_on_nested_class()
        => ExecutionTest("private-getter-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-getter-on-nested-class.js")]
    public Task private_getter_shadowed_by_getter_on_nested_class()
        => ExecutionTest("private-getter-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-method-on-nested-class.js")]
    public Task private_getter_shadowed_by_method_on_nested_class()
        => ExecutionTest("private-getter-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-setter-on-nested-class.js")]
    public Task private_getter_shadowed_by_setter_on_nested_class()
        => ExecutionTest("private-getter-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-method-access-on-inner-arrow-function.js")]
    public Task private_method_access_on_inner_arrow_function()
        => ExecutionTest("private-method-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-method-access-on-inner-function.js")]
    public Task private_method_access_on_inner_function()
        => ExecutionTest("private-method-access-on-inner-function");

    [Fact(DisplayName = "private-method-brand-check.js")]
    public Task private_method_brand_check()
        => ExecutionTest("private-method-brand-check");

    [Fact(DisplayName = "private-method-get-and-call.js")]
    public Task private_method_get_and_call()
        => ExecutionTest("private-method-get-and-call");

    [Fact(DisplayName = "private-method-on-nested-class.js")]
    public Task private_method_on_nested_class()
        => ExecutionTest("private-method-on-nested-class");

    [Fact(DisplayName = "private-method-referenced-from-static-method.js")]
    public Task private_method_referenced_from_static_method()
        => ExecutionTest("private-method-referenced-from-static-method");

    [Fact(DisplayName = "private-method-shadowed-by-getter-on-nested-class.js")]
    public Task private_method_shadowed_by_getter_on_nested_class()
        => ExecutionTest("private-method-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-method-shadowed-by-setter-on-nested-class.js")]
    public Task private_method_shadowed_by_setter_on_nested_class()
        => ExecutionTest("private-method-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-setter-access-on-inner-arrow-function.js")]
    public Task private_setter_access_on_inner_arrow_function()
        => ExecutionTest("private-setter-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-setter-on-nested-class.js")]
    public Task private_setter_on_nested_class()
        => ExecutionTest("private-setter-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-getter-on-nested-class.js")]
    public Task private_static_field_shadowed_by_getter_on_nested_class()
        => ExecutionTest("private-static-field-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-method-on-nested-class.js")]
    public Task private_static_field_shadowed_by_method_on_nested_class()
        => ExecutionTest("private-static-field-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-static-field-usage-inside-nested-class.js")]
    public Task private_static_field_usage_inside_nested_class()
        => ExecutionTest("private-static-field-usage-inside-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-field-on-nested-class.js")]
    public Task private_static_method_shadowed_by_field_on_nested_class()
        => ExecutionTest("private-static-method-shadowed-by-field-on-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-getter-on-nested-class.js")]
    public Task private_static_method_shadowed_by_getter_on_nested_class()
        => ExecutionTest("private-static-method-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-method-on-nested-class.js")]
    public Task private_static_method_shadowed_by_method_on_nested_class()
        => ExecutionTest("private-static-method-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-static-method-shadowed-by-setter-on-nested-class.js")]
    public Task private_static_method_shadowed_by_setter_on_nested_class()
        => ExecutionTest("private-static-method-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-static-method-usage-inside-nested-class.js")]
    public Task private_static_method_usage_inside_nested_class()
        => ExecutionTest("private-static-method-usage-inside-nested-class");

    [Fact(DisplayName = "privatefield-on-proxy.js")]
    public Task privatefield_on_proxy()
        => ExecutionTest("privatefield-on-proxy");

    [Fact(DisplayName = "privatefieldget-primitive-receiver.js")]
    public Task privatefieldget_primitive_receiver()
        => ExecutionTest("privatefieldget-primitive-receiver");

    [Fact(DisplayName = "privatefieldget-success-1.js")]
    public Task privatefieldget_success_1()
        => ExecutionTest("privatefieldget-success-1");

    [Fact(DisplayName = "privatefieldget-typeerror-2.js")]
    public Task privatefieldget_typeerror_2()
        => ExecutionTest("privatefieldget-typeerror-2");

    [Fact(DisplayName = "privatefieldget-typeerror-3.js")]
    public Task privatefieldget_typeerror_3()
        => ExecutionTest("privatefieldget-typeerror-3");

    [Fact(DisplayName = "privatefieldput-primitive-receiver.js")]
    public Task privatefieldput_primitive_receiver()
        => ExecutionTest("privatefieldput-primitive-receiver");

    [Fact(DisplayName = "privatefieldset-evaluation-order-2.js")]
    public Task privatefieldset_evaluation_order_2()
        => ExecutionTest("privatefieldset-evaluation-order-2");

    [Fact(DisplayName = "privatefieldset-evaluation-order-3.js")]
    public Task privatefieldset_evaluation_order_3()
        => ExecutionTest("privatefieldset-evaluation-order-3");

    [Fact(DisplayName = "privatefieldset-typeerror-10.js")]
    public Task privatefieldset_typeerror_10()
        => ExecutionTest("privatefieldset-typeerror-10");

    [Fact(DisplayName = "privatefieldset-typeerror-11.js")]
    public Task privatefieldset_typeerror_11()
        => ExecutionTest("privatefieldset-typeerror-11");

    [Fact(DisplayName = "privatefieldset-typeerror-2.js")]
    public Task privatefieldset_typeerror_2()
        => ExecutionTest("privatefieldset-typeerror-2");

    [Fact(DisplayName = "privatefieldset-typeerror-3.js")]
    public Task privatefieldset_typeerror_3()
        => ExecutionTest("privatefieldset-typeerror-3");

    [Fact(DisplayName = "privatefieldset-typeerror-6.js")]
    public Task privatefieldset_typeerror_6()
        => ExecutionTest("privatefieldset-typeerror-6");

    [Fact(DisplayName = "privatefieldset-typeerror-7.js")]
    public Task privatefieldset_typeerror_7()
        => ExecutionTest("privatefieldset-typeerror-7");

    [Fact(DisplayName = "privatefieldset-typeerror-8.js")]
    public Task privatefieldset_typeerror_8()
        => ExecutionTest("privatefieldset-typeerror-8");

    [Fact(DisplayName = "privatefieldset-typeerror-9.js")]
    public Task privatefieldset_typeerror_9()
        => ExecutionTest("privatefieldset-typeerror-9");

    [Fact(DisplayName = "privatename-valid-no-earlyerr.js")]
    public Task privatename_valid_no_earlyerr()
        => ExecutionTest("privatename-valid-no-earlyerr");

    [Fact(DisplayName = "public-class-field-initialization-on-super-class-with-setter.js")]
    public Task public_class_field_initialization_on_super_class_with_setter()
        => ExecutionTest("public-class-field-initialization-on-super-class-with-setter");

    [Fact(DisplayName = "redeclaration.js")]
    public Task redeclaration()
        => ExecutionTest("redeclaration");

    [Fact(DisplayName = "regular-definitions-computed-names.js")]
    public Task regular_definitions_computed_names()
        => ExecutionTest("regular-definitions-computed-names");

    [Fact(DisplayName = "regular-definitions-literal-names-asi.js")]
    public Task regular_definitions_literal_names_asi()
        => ExecutionTest("regular-definitions-literal-names-asi");

    [Fact(DisplayName = "regular-definitions-literal-names.js")]
    public Task regular_definitions_literal_names()
        => ExecutionTest("regular-definitions-literal-names");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task regular_definitions_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("regular-definitions-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-by-classname.js")]
    public Task regular_definitions_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("regular-definitions-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task regular_definitions_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("regular-definitions-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "regular-definitions-static-private-fields.js")]
    public Task regular_definitions_static_private_fields()
        => ExecutionTest("regular-definitions-static-private-fields");

    [Fact(DisplayName = "regular-definitions-static-private-methods-with-fields.js")]
    public Task regular_definitions_static_private_methods_with_fields()
        => ExecutionTest("regular-definitions-static-private-methods-with-fields");

    [Fact(DisplayName = "same-line-gen-computed-names.js")]
    public Task same_line_gen_computed_names()
        => ExecutionTest("same-line-gen-computed-names");

    [Fact(DisplayName = "same-line-gen-literal-names-asi.js")]
    public Task same_line_gen_literal_names_asi()
        => ExecutionTest("same-line-gen-literal-names-asi");

    [Fact(DisplayName = "same-line-gen-literal-names.js")]
    public Task same_line_gen_literal_names()
        => ExecutionTest("same-line-gen-literal-names");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task same_line_gen_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-alt.js")]
    public Task same_line_gen_rs_static_privatename_identifier_alt()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-by-classname.js")]
    public Task same_line_gen_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task same_line_gen_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task same_line_gen_rs_static_privatename_identifier_initializer_alt()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier-initializer.js")]
    public Task same_line_gen_rs_static_privatename_identifier_initializer()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "same-line-gen-rs-static-privatename-identifier.js")]
    public Task same_line_gen_rs_static_privatename_identifier()
        => ExecutionTest("same-line-gen-rs-static-privatename-identifier");

    [Fact(DisplayName = "same-line-gen-static-private-fields.js")]
    public Task same_line_gen_static_private_fields()
        => ExecutionTest("same-line-gen-static-private-fields");

    [Fact(DisplayName = "same-line-gen-static-private-methods-with-fields.js")]
    public Task same_line_gen_static_private_methods_with_fields()
        => ExecutionTest("same-line-gen-static-private-methods-with-fields");

    [Fact(DisplayName = "same-line-method-computed-names.js")]
    public Task same_line_method_computed_names()
        => ExecutionTest("same-line-method-computed-names");

    [Fact(DisplayName = "same-line-method-literal-names-asi.js")]
    public Task same_line_method_literal_names_asi()
        => ExecutionTest("same-line-method-literal-names-asi");

    [Fact(DisplayName = "same-line-method-literal-names.js")]
    public Task same_line_method_literal_names()
        => ExecutionTest("same-line-method-literal-names");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task same_line_method_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-alt.js")]
    public Task same_line_method_rs_static_privatename_identifier_alt()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-by-classname.js")]
    public Task same_line_method_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task same_line_method_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task same_line_method_rs_static_privatename_identifier_initializer_alt()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier-initializer.js")]
    public Task same_line_method_rs_static_privatename_identifier_initializer()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "same-line-method-rs-static-privatename-identifier.js")]
    public Task same_line_method_rs_static_privatename_identifier()
        => ExecutionTest("same-line-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "same-line-method-static-private-fields.js")]
    public Task same_line_method_static_private_fields()
        => ExecutionTest("same-line-method-static-private-fields");

    [Fact(DisplayName = "same-line-method-static-private-methods-with-fields.js")]
    public Task same_line_method_static_private_methods_with_fields()
        => ExecutionTest("same-line-method-static-private-methods-with-fields");

    [Fact(DisplayName = "set-access-of-missing-shadowed-private-setter.js")]
    public Task set_access_of_missing_shadowed_private_setter()
        => ExecutionTest("set-access-of-missing-shadowed-private-setter");

    [Fact(DisplayName = "set-access-of-shadowed-private-method.js")]
    public Task set_access_of_shadowed_private_method()
        => ExecutionTest("set-access-of-shadowed-private-method");

    [Fact(DisplayName = "static-as-valid-instance-field-assigned.js")]
    public Task static_as_valid_instance_field_assigned()
        => ExecutionTest("static-as-valid-instance-field-assigned");

    [Fact(DisplayName = "static-as-valid-instance-field.js")]
    public Task static_as_valid_instance_field()
        => ExecutionTest("static-as-valid-instance-field");

    [Fact(DisplayName = "static-as-valid-static-field-assigned.js")]
    public Task static_as_valid_static_field_assigned()
        => ExecutionTest("static-as-valid-static-field-assigned");

    [Fact(DisplayName = "static-as-valid-static-field.js")]
    public Task static_as_valid_static_field()
        => ExecutionTest("static-as-valid-static-field");

    [Fact(DisplayName = "static-field-anonymous-function-length.js")]
    public Task static_field_anonymous_function_length()
        => ExecutionTest("static-field-anonymous-function-length");

    [Fact(DisplayName = "static-field-anonymous-function-name.js")]
    public Task static_field_anonymous_function_name()
        => ExecutionTest("static-field-anonymous-function-name");

    [Fact(DisplayName = "static-field-declaration.js")]
    public Task static_field_declaration()
        => ExecutionTest("static-field-declaration");

    [Fact(DisplayName = "static-field-init-this-inside-arrow-function.js")]
    public Task static_field_init_this_inside_arrow_function()
        => ExecutionTest("static-field-init-this-inside-arrow-function");

    [Fact(DisplayName = "static-field-redeclaration.js")]
    public Task static_field_redeclaration()
        => ExecutionTest("static-field-redeclaration");

    [Fact(DisplayName = "static-private-fields-proxy-default-handler-throws.js")]
    public Task static_private_fields_proxy_default_handler_throws()
        => ExecutionTest("static-private-fields-proxy-default-handler-throws");

    [Fact(DisplayName = "static-private-getter-access-on-inner-arrow-function.js")]
    public Task static_private_getter_access_on_inner_arrow_function()
        => ExecutionTest("static-private-getter-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-private-getter-access-on-inner-class.js")]
    public Task static_private_getter_access_on_inner_class()
        => ExecutionTest("static-private-getter-access-on-inner-class");

    [Fact(DisplayName = "static-private-getter-access-on-inner-function.js")]
    public Task static_private_getter_access_on_inner_function()
        => ExecutionTest("static-private-getter-access-on-inner-function");

    [Fact(DisplayName = "static-private-method-access-on-inner-arrow-function.js")]
    public Task static_private_method_access_on_inner_arrow_function()
        => ExecutionTest("static-private-method-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-private-method-access-on-inner-function.js")]
    public Task static_private_method_access_on_inner_function()
        => ExecutionTest("static-private-method-access-on-inner-function");

    [Fact(DisplayName = "static-private-method-referenced-from-instance-method.js")]
    public Task static_private_method_referenced_from_instance_method()
        => ExecutionTest("static-private-method-referenced-from-instance-method");

    [Fact(DisplayName = "static-private-setter-access-on-inner-arrow-function.js")]
    public Task static_private_setter_access_on_inner_arrow_function()
        => ExecutionTest("static-private-setter-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-private-setter-access-on-inner-class.js")]
    public Task static_private_setter_access_on_inner_class()
        => ExecutionTest("static-private-setter-access-on-inner-class");

    [Fact(DisplayName = "super-access-from-arrow-func-on-field.js")]
    public Task super_access_from_arrow_func_on_field()
        => ExecutionTest("super-access-from-arrow-func-on-field");

    [Fact(DisplayName = "super-access-inside-a-private-method.js")]
    public Task super_access_inside_a_private_method()
        => ExecutionTest("super-access-inside-a-private-method");

    [Fact(DisplayName = "wrapped-in-sc-computed-names.js")]
    public Task wrapped_in_sc_computed_names()
        => ExecutionTest("wrapped-in-sc-computed-names");

    [Fact(DisplayName = "wrapped-in-sc-literal-names-asi.js")]
    public Task wrapped_in_sc_literal_names_asi()
        => ExecutionTest("wrapped-in-sc-literal-names-asi");

    [Fact(DisplayName = "wrapped-in-sc-literal-names.js")]
    public Task wrapped_in_sc_literal_names()
        => ExecutionTest("wrapped-in-sc-literal-names");

    [Fact(DisplayName = "wrapped-in-sc-rs-privatename-identifier-initializer-alt.js")]
    public Task wrapped_in_sc_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("wrapped-in-sc-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-privatename-identifier-initializer.js")]
    public Task wrapped_in_sc_rs_privatename_identifier_initializer()
        => ExecutionTest("wrapped-in-sc-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "wrapped-in-sc-rs-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-privatename-identifier");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task wrapped_in_sc_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-generator-method-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-method-privatename-identifier-alt.js")]
    public Task wrapped_in_sc_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-method-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_static_method_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-alt-by-classname.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_alt_by_classname()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-alt-by-classname");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-alt.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-by-classname.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_by_classname()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-by-classname");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-initializer-alt-by-classname.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_initializer_alt_by_classname()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-initializer-alt-by-classname");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-initializer-alt.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_initializer_alt()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier-initializer.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier_initializer()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "wrapped-in-sc-rs-static-privatename-identifier.js")]
    public Task wrapped_in_sc_rs_static_privatename_identifier()
        => ExecutionTest("wrapped-in-sc-rs-static-privatename-identifier");

    [Fact(DisplayName = "wrapped-in-sc-static-private-fields.js")]
    public Task wrapped_in_sc_static_private_fields()
        => ExecutionTest("wrapped-in-sc-static-private-fields");

    [Fact(DisplayName = "wrapped-in-sc-static-private-methods-with-fields.js")]
    public Task wrapped_in_sc_static_private_methods_with_fields()
        => ExecutionTest("wrapped-in-sc-static-private-methods-with-fields");

    [Fact(DisplayName = "wrapped-in-sc-static-private-methods.js")]
    public Task wrapped_in_sc_static_private_methods()
        => ExecutionTest("wrapped-in-sc-static-private-methods");
}
